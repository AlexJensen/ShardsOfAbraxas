using Abraxas.Events;
using Abraxas.Stones.Data;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Abraxas.Stones.Targets
{
    [CreateAssetMenu(fileName = "New Target Property Of Target", menuName = "Abraxas/Data/StoneData/Targets/Target Property Of Target")]
    class Target_PropertyOfTargetSO : TargetSO<object>
    {
        public TargetSOBase _targetObject; // The target object whose property we want to access
        public string _propertyName; // The name of the property we want to access
        string _expectedTypeName; // The expected type of the property we want to access

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
            if (targetObject is Target_PropertyOfTargetSO nestedPropertyTarget)
            {
                return ResolveTargetType(nestedPropertyTarget._targetObject).GetProperty(nestedPropertyTarget._propertyName).PropertyType;
            }
            return targetObject.ResolveRuntimeType();
        }

        public void SetResolvedType(Type t)
        {
            // We store it in _expectedTypeName or a similar field
            if (t == null)
            {
                _expectedTypeName = string.Empty;
            }
            else
            {
                _expectedTypeName = t.AssemblyQualifiedName;
            }
        }

        public override Type ResolveRuntimeType()
        {
            // If we stored the discovered property type in _expectedTypeName:
            if (!string.IsNullOrEmpty(_expectedTypeName))
            {
                var resolved = Type.GetType(_expectedTypeName);
                if (resolved != null) return resolved;
            }
            // Otherwise fall back
            return base.ResolveRuntimeType();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Target_PropertyOfTargetSO))]
    public class Target_PropertyOfTargetEditor : StoneSOEditor
    {
        private SerializedProperty _targetProperty;
        private SerializedProperty _propertyNameProperty;
        private string[] _propertyNames;
        private PropertyInfo[] _filteredProperties;
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

            var propertyTarget = (Target_PropertyOfTargetSO)target;

            if (_targetProperty.objectReferenceValue != null)
            {
                UpdatePropertyList();

                if (_propertyNames.Length > 0)
                {
                    _selectedPropertyIndex = Array.IndexOf(_propertyNames, _propertyNameProperty.stringValue);
                    if (_selectedPropertyIndex == -1) _selectedPropertyIndex = 0;

                    _selectedPropertyIndex = EditorGUILayout.Popup("Property", _selectedPropertyIndex, _propertyNames);
                    _propertyNameProperty.stringValue = _propertyNames[_selectedPropertyIndex];
                    var chosenProperty = _filteredProperties[_selectedPropertyIndex];
                    Type chosenPropertyType = chosenProperty.PropertyType;

                    var targetPropObj = (Target_PropertyOfTargetSO)target;
                    if (chosenPropertyType != null)
                    {
                        // Call a method on the SO that sets its “runtime type” for the next level
                        targetPropObj.SetResolvedType(chosenPropertyType);
                    }
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
                    Type expectedType = (target as TargetSOBase)?.ExpectedType;
                    ShowSelectTypeMenu<TargetSOBase>(type => SetTarget(type, _targetProperty, expectedType));
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
                    const BindingFlags flags = BindingFlags.Instance
                                      | BindingFlags.Public
                                      | BindingFlags.NonPublic
                                      | BindingFlags.FlattenHierarchy;
                    var properties = targetType.GetProperties(flags);
                    Type expectedType = (target as TargetSOBase)?.ExpectedType;
                    if (expectedType != null)
                    {
                        properties = properties.Where(p => expectedType.IsAssignableFrom(p.PropertyType)).ToArray();
                    }
                    _filteredProperties = properties;
                    _propertyNames = properties.Select(p => p.Name).ToArray();
                }
            }
            else
            {
                _filteredProperties = Array.Empty<PropertyInfo>();
                _propertyNames = new string[0];
            }
        }

        private Type ResolveTargetType(TargetSOBase targetObject)
        {
            return targetObject.ResolveRuntimeType();
        }

        
    }
#endif
}
