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
        protected void ShowSelectTypeMenu<T>(Action<Type> onSelected)
        {
            var menu = new GenericMenu();
            var types = Assembly.GetAssembly(typeof(T))
                    .GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && typeof(T).IsAssignableFrom(t) && typeof(ScriptableObject).IsAssignableFrom(t))
                    .ToList();
            foreach (var type in types)
            {
                //if (CanProduceType(type, expectedType))
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

        protected void RemoveTarget(SerializedProperty property)
        {
            if (property.objectReferenceValue != null)
            {
                var target = property.objectReferenceValue as ScriptableObject;
                if (target != null)
                {
                    RecursiveRemoveObject(target);
                }
                property.objectReferenceValue = null;
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void RecursiveRemoveObject(ScriptableObject so)
        {
            if (so == null) return;

            // Mark for Undo
            Undo.RecordObject(so, "Remove SO");

            // 1) Use reflection to find any ScriptableObject fields
            var soType = so.GetType();

            // We'll consider both single fields and lists
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic |
                               BindingFlags.Instance | BindingFlags.DeclaredOnly;

            // For each field in the object:
            foreach (var field in soType.GetFields(bindingFlags))
            {
                // Only proceed if it’s not a primitive type, etc.
                if (typeof(ScriptableObject).IsAssignableFrom(field.FieldType))
                {
                    // Single ScriptableObject field
                    var childSO = field.GetValue(so) as ScriptableObject;
                    if (childSO != null)
                    {
                        RecursiveRemoveObject(childSO);
                    }
                }
                else if (typeof(System.Collections.IList).IsAssignableFrom(field.FieldType))
                {
                    if (field.GetValue(so) is not System.Collections.IList listObj) continue;

                    // Iterate each item
                    foreach (var item in listObj)
                    {
                        if (item is ScriptableObject child)
                        {
                            RecursiveRemoveObject(child);
                        }
                    }
                }
                // else: other field type => ignore
            }

            // 2) Finally, remove this object from the asset
            AssetDatabase.RemoveObjectFromAsset(so);
            DestroyImmediate(so, true);
            AssetDatabase.SaveAssets();
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