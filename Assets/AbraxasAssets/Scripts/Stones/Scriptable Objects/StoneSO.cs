using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Abraxas.Events;
using System.Collections.Generic;
using Abraxas.Events.Managers;
using Zenject;

namespace Abraxas.Stones.Data
{
    public abstract class StoneSO : ScriptableObject
    {
        public abstract IStoneData Data { get; set; }
        public abstract Type ControllerType { get; set; }
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
                var typeName = type.Name;
                if (type.IsGenericTypeDefinition)
                {
                    typeName += "<T>"; // Indicate that this is a generic type
                }
                menu.AddItem(new GUIContent(typeName), false, () => onSelected(type));
            }
            menu.ShowAsContext();
        }

        protected void SetTarget(Type type, SerializedProperty property)
        {
            RemoveTarget(property);
            CreateNewTarget(type, property);

            // Check if the created target requires additional configuration
            if (typeof(Target_TriggeringObject).IsAssignableFrom(type))
            {
                ShowSelectEventTypeMenu(property);
            }
        }

        protected void CreateNewTarget(Type type, SerializedProperty property)
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
                            (target as Target_TriggeringObject).SetEventType(type);
                        });
                    }
                }
            }
            menu.ShowAsContext();
        }

    }

#endif
}