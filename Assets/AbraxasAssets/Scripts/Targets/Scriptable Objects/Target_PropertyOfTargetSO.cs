using Abraxas.Events;
using Abraxas.Stones.Data;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Abraxas.Stones.Targets
{
    [CreateAssetMenu(fileName = "New Target Property Of Target", menuName = "Abraxas/Data/StoneData/Targets/Target Property Of Target")]
    class Target_PropertyOfTargetSO : TargetSO<object>
    {
        public TargetSOBase _targetObject; // The target object whose property we want to access
        public string _propertyName; // The name of the property we want to access

        public override object Target
        {
            get
            {
                if (_targetObject == null || string.IsNullOrEmpty(_propertyName))
                {
                    return null;
                }

                Type targetType = ResolveTargetType(_targetObject);
                var propertyInfo = targetType.GetProperty(_propertyName);

                if (propertyInfo == null)
                {
                    Debug.LogError($"Property {_propertyName} not found on target of type {targetType}");
                    return null;
                }

                return propertyInfo.GetValue(_targetObject.GetTarget());
            }
            set
            {
                if (_targetObject == null || string.IsNullOrEmpty(_propertyName))
                {
                    return;
                }

                Type targetType = ResolveTargetType(_targetObject);
                var propertyInfo = targetType.GetProperty(_propertyName);

                if (propertyInfo == null || !propertyInfo.CanWrite)
                {
                    Debug.LogError($"Property {_propertyName} not found or not writable on target of type {targetType}");
                    return;
                }

                propertyInfo.SetValue(_targetObject.GetTarget(), value);
            }
        }

        private Type ResolveTargetType(TargetSOBase targetObject)
        {
            // Special handling for Target_TriggeringObject
            if (targetObject is Target_TriggeringObjectSO triggeringObject)
            {
                return triggeringObject.EventType ?? typeof(IEvent); // Fallback to IEvent if type not found
            }

            // Handling for Target_PropertyOfTarget and other nested targets
            if (targetObject is Target_PropertyOfTargetSO nestedPropertyTarget)
            {
                return ResolveTargetType(nestedPropertyTarget._targetObject).GetProperty(nestedPropertyTarget._propertyName).PropertyType;
            }

            // General case
            return targetObject.GetType().BaseType.GetGenericArguments()[0];
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Target_PropertyOfTargetSO))]
    public class Target_PropertyOfTargetEditor : StoneSOEditor
    {
        private SerializedProperty _targetProperty;
        private SerializedProperty _propertyNameProperty;
        private string[] _propertyNames;
        private int _selectedPropertyIndex;

        protected void OnEnable()
        {
            _targetProperty = serializedObject.FindProperty("_targetObject");
            _propertyNameProperty = serializedObject.FindProperty("_propertyName");

            UpdatePropertyList();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_targetProperty);

            if (_targetProperty.objectReferenceValue != null)
            {
                UpdatePropertyList();

                if (_propertyNames.Length > 0)
                {
                    _selectedPropertyIndex = Array.IndexOf(_propertyNames, _propertyNameProperty.stringValue);
                    if (_selectedPropertyIndex == -1) _selectedPropertyIndex = 0;

                    _selectedPropertyIndex = EditorGUILayout.Popup("Property", _selectedPropertyIndex, _propertyNames);
                    _propertyNameProperty.stringValue = _propertyNames[_selectedPropertyIndex];
                }

                if (GUILayout.Button("Remove Target"))
                {
                    RemoveTarget(_targetProperty);
                }
            }
            else
            {
                if (GUILayout.Button("Set Target"))
                {
                    ShowSelectTypeMenu<TargetSOBase>(type => SetTarget(type, _targetProperty));
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void UpdatePropertyList()
        {
            if (_targetProperty.objectReferenceValue != null)
            {
                var targetObject = (_targetProperty.objectReferenceValue as TargetSOBase);
                if (targetObject != null)
                {
                    Type targetType = ResolveTargetType(targetObject);
                    var properties = targetType.GetProperties();
                    _propertyNames = properties.Select(p => p.Name).ToArray();
                }
            }
            else
            {
                _propertyNames = new string[0];
            }
        }

        private Type ResolveTargetType(TargetSOBase targetObject)
        {
            if (targetObject is Target_TriggeringObjectSO triggeringObject)
            {
                return triggeringObject.EventType ?? typeof(IEvent);
            }

            if (targetObject is Target_PropertyOfTargetSO nestedPropertyTarget && nestedPropertyTarget._targetObject != null && !string.IsNullOrEmpty(nestedPropertyTarget._propertyName))
            {
                return ResolveTargetType(nestedPropertyTarget._targetObject).GetProperty(nestedPropertyTarget._propertyName).PropertyType;
            }

            return targetObject.GetType().BaseType.GetGenericArguments()[0];
        }
    }
#endif
}
