using Abraxas.Events;
using Abraxas.Stones.Targets;
using System;
using System.Linq;
using System.Reflection;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace Abraxas.Stones.Data
{
    public abstract class StoneSO : ScriptableObject
    {
        public abstract IStoneData Data { get; set; }
        public abstract Type ControllerType { get; set; }

        public virtual void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            Data.NetworkSerialize(serializer);
        }
    }    

#if UNITY_EDITOR
    [CustomEditor(typeof(StoneSO), true)]
    public class StoneSOEditor : Editor
    {
        protected void ShowSelectTypeMenu<T>(Type expectedType, Action<Type> onSelected)
        {
            var menu = new GenericMenu();
            var types = Assembly.GetAssembly(typeof(T))
                    .GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && typeof(T).IsAssignableFrom(t) && typeof(ScriptableObject).IsAssignableFrom(t))
                    .ToList();
            foreach (var type in types)
            {
                if (CanProduceType(type, expectedType))
                {
                    var typeName = type.Name;
                    if (type.IsGenericTypeDefinition)
                    {
                        typeName += "<T>"; // Indicate that this is a generic type
                    }
                    menu.AddItem(new GUIContent(typeName), false, () => onSelected(type));
                }
            }
            if (menu.GetItemCount() == 0)
            {
                menu.AddDisabledItem(new GUIContent("No Compatible Types Found"));
            }
            menu.ShowAsContext();
        }
        private bool CanProduceType(Type targetType, Type expectedType)
        {
            var baseType = targetType.BaseType;
            if (baseType != null && baseType.IsGenericType)
            {
                var returnType = baseType.GetGenericArguments()[0];

                // If returnType is object, treat it as a wildcard and allow selection
                if (returnType == typeof(object))
                {
                    return true;
                }

                return expectedType == null || expectedType.IsAssignableFrom(returnType);
            }

            return expectedType == null;
        }


        protected void SetTarget(Type type, SerializedProperty property, Type expectedType = null)
        {
            RemoveTarget(property);
            CreateNewTarget(type, property, expectedType);

            if (typeof(Target_TriggeringObjectSO).IsAssignableFrom(type))
            {
                ShowSelectEventTypeMenu(property);
            }
        }

        protected void CreateNewTarget(Type type, SerializedProperty property, Type expectedType)
        {
            var parent = (ScriptableObject)property.serializedObject.targetObject;
            ScriptableObject target;
            if (type.IsGenericTypeDefinition)
            {
                var genericArguments = new Type[] { typeof(object) };
                var closedType = type.MakeGenericType(genericArguments);
                target = (ScriptableObject)Activator.CreateInstance(closedType);
            }
            else
            {
                target = CreateInstance(type);
            }
            target.name = type.Name;
            AssetDatabase.AddObjectToAsset(target, parent);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(parent));

            property.objectReferenceValue = target;
            serializedObject.ApplyModifiedProperties();

            if (expectedType != null && target is TargetSOBase targetSO)
            {
                targetSO.SetExpectedType(expectedType);
                EditorUtility.SetDirty(targetSO);
            }
        }

        protected virtual Type GetExpectedTypeFromEffect(StoneSO target)
        {
            Type effectType = target.ControllerType;

            // Check if effectType implements ITargetable<>
            var iTargetableInterface = effectType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ITargetable<>));

            if (iTargetableInterface != null)
            {
                Type expectedType = iTargetableInterface.GetGenericArguments()[0];
                // Now you know the expectedType that the effect wants.
                return expectedType;
            }
            return null;
        }

        protected void RemoveTarget(SerializedProperty property)
        {
            if (property.objectReferenceValue != null)
            {
                var target = property.objectReferenceValue as ScriptableObject;
                if (target != null)
                {
                    Undo.RecordObject(target, "Remove Target");
                    AssetDatabase.RemoveObjectFromAsset(target);
                    DestroyImmediate(target, true);
                    AssetDatabase.SaveAssets();
                }
                property.objectReferenceValue = null;
                serializedObject.ApplyModifiedProperties();
            }
        }

        protected void ShowSelectEventTypeMenu(SerializedProperty property)
        {
            var menu = new GenericMenu();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (typeof(IEvent).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                    {
                        menu.AddItem(new GUIContent(type.FullName), false, () => {
                            property.stringValue = type.AssemblyQualifiedName;
                            serializedObject.ApplyModifiedProperties();
                            (target as Target_TriggeringObjectSO).SetEventType(type);
                        });
                    }
                }
            }
            menu.ShowAsContext();
        }
    }
#endif
}