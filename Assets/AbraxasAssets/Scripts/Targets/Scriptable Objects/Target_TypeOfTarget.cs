﻿using Abraxas.Events;
using Abraxas.Stones.Data;
using System;
using UnityEditor;
using UnityEngine;

namespace Abraxas.Stones.Targets
{
    [CreateAssetMenu(fileName = "New Type Of Target", menuName = "Abraxas/Data/StoneData/Targets/TypeOfTarget")]
    class TypeOfTargetSO : TargetSO<Type>
    {
        [SerializeField]
        private TargetSOBase _targetObject; // The target from which we derive the type

        public override Type Target
        {
            get
            {
                if (_targetObject == null)
                {
                    return null;
                }

                object targetValue = _targetObject.GetTarget();
                if (targetValue == null)
                {
                    return null;
                }

                return targetValue.GetType();
            }
            set
            {
                // This does nothing since the type is derived dynamically
                Debug.LogWarning("TypeOfTargetSO does not support setting the target type directly.");
            }
        }
    }

    [CustomEditor(typeof(TypeOfTargetSO))]
    public class TypeOfTargetEditor : StoneSOEditor
    {
        private SerializedProperty _targetProperty;

        protected void OnEnable()
        {
            _targetProperty = serializedObject.FindProperty("_targetObject");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_targetProperty, new GUIContent("Target Object"));

            if (_targetProperty.objectReferenceValue == null)
            {
                if (GUILayout.Button("Set Target"))
                {
                    ShowSelectTypeMenu<TargetSOBase>(type => SetTarget(type, _targetProperty));
                }
            }
            else
            {
                // Display what we believe the type will be.
                var targetObject = _targetProperty.objectReferenceValue as TargetSOBase;
                if (targetObject != null)
                {
                    Type resolvedType = ResolveTargetType(targetObject);
                    EditorGUILayout.LabelField("Resolved Runtime Type:", resolvedType != null ? resolvedType.FullName : "Unknown");

                    if (GUILayout.Button("Remove Target"))
                    {
                        RemoveTarget(_targetProperty);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private Type ResolveTargetType(TargetSOBase targetObject)
        {
            if (targetObject is Target_TriggeringObjectSO triggeringObject)
            {
                return triggeringObject.EventType ?? typeof(IEvent);
            }

            if (targetObject is Target_PropertyOfTargetSO nestedPropertyTarget
                && nestedPropertyTarget._targetObject != null
                && !string.IsNullOrEmpty(nestedPropertyTarget._propertyName))
            {
                Type nestedType = ResolveTargetType(nestedPropertyTarget._targetObject);
                if (nestedType != null)
                {
                    var propertyInfo = nestedType.GetProperty(nestedPropertyTarget._propertyName);
                    if (propertyInfo != null)
                        return propertyInfo.PropertyType;
                }
                return null;
            }

            // General case: Get the generic type argument of targetObject's parent class (TargetSO<T>)
            Type baseType = targetObject.GetType().BaseType;
            if (baseType != null && baseType.IsGenericType)
            {
                return baseType.GetGenericArguments()[0];
            }

            return null;
        }
    }
}
