using Abraxas.Stones.Conditions;
using Abraxas.Stones.EventListeners;
using Abraxas.Stones.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Abraxas.Stones.Data
{
    [CreateAssetMenu(fileName = "New Trigger", menuName = "Abraxas/Data/StoneData/Trigger")]
    public class TriggerStoneSO : StoneSO
    {
        #region Fields
        public List<int> ConnectionIndexes = new();

        [SerializeField]
        List<ScriptableObject> _conditions = new();

        [SerializeField]
        List<ScriptableObject> _eventListeners = new();

        [SerializeField]
        BasicStoneData _data = new();
        #endregion

        #region Properties
        public List<ScriptableObject> Conditions { get => _conditions; set => _conditions = value; }
        public List<ScriptableObject> EventListeners { get => _eventListeners; set => _eventListeners = value; }
        public override Type ControllerType { get; set; } = typeof(TriggerStone);
        public override IStoneData Data { get => _data; set => _data = (BasicStoneData)value; }
        #endregion

        #region Serialization
        public override void NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            // Serialize ConnectionIndexes
            int indexCount = ConnectionIndexes != null ? ConnectionIndexes.Count : 0;
            serializer.SerializeValue(ref indexCount);

            if (serializer.IsReader)
            {
                ConnectionIndexes = new List<int>(indexCount);
                for (int i = 0; i < indexCount; i++)
                {
                    int index = 0;
                    serializer.SerializeValue(ref index);
                    ConnectionIndexes.Add(index);
                }
            }
            else
            {
                foreach (var index in ConnectionIndexes)
                {
                    int tempIndex = index;
                    serializer.SerializeValue(ref tempIndex);
                }
            }

            // Serialize Conditions
            SerializeScriptableObjectList(serializer, ref _conditions);

            // Serialize Event Listeners (using EventListenerSOBase)
            SerializeScriptableObjectList(serializer, ref _eventListeners);

            // Serialize Data
            Data.NetworkSerialize(serializer);
        }

        private void SerializeScriptableObjectList<T>(BufferSerializer<T> serializer, ref List<ScriptableObject> list) where T : IReaderWriter
        {
            int count = list != null ? list.Count : 0;
            serializer.SerializeValue(ref count);

            if (serializer.IsReader)
            {
                list = new List<ScriptableObject>(count);
                for (int i = 0; i < count; i++)
                {
                    string typeId = string.Empty;
                    serializer.SerializeValue(ref typeId);

                    var scriptableObjectType = Type.GetType(typeId);
                    if (scriptableObjectType != null)
                    {
                        var obj = CreateInstance(scriptableObjectType);
                        if (obj is INetworkSerializable serializableObj)
                        {
                            serializableObj.NetworkSerialize(serializer);
                        }
                        list.Add(obj);
                    }
                }
            }
            else
            {
                foreach (var item in list)
                {
                    string typeId = item.GetType().AssemblyQualifiedName;
                    serializer.SerializeValue(ref typeId);

                    if (item is INetworkSerializable serializableObj)
                    {
                        serializableObj.NetworkSerialize(serializer);
                    }
                }
            }
        }
        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TriggerStoneSO))]
    public class TriggerStoneSOEditor : Editor
    {
        private SerializedProperty _conditionsProperty;
        private SerializedProperty _eventListenersProperty;
        private SerializedProperty _connectionIndexesProperty;
        private SerializedProperty _dataProperty;

        private void OnEnable()
        {
            _conditionsProperty = serializedObject.FindProperty("_conditions");
            _eventListenersProperty = serializedObject.FindProperty("_eventListeners");
            _connectionIndexesProperty = serializedObject.FindProperty("ConnectionIndexes");
            _dataProperty = serializedObject.FindProperty("_data");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Trigger Stone Settings", EditorStyles.boldLabel);

            // Display BasicStoneData
            EditorGUILayout.LabelField("Basic Stone Data", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_dataProperty, new GUIContent("Stone Data"), true);

            // Display ConnectionIndexes
            EditorGUILayout.LabelField("Connection Indexes", EditorStyles.boldLabel);
            for (int i = 0; i < _connectionIndexesProperty.arraySize; i++)
            {
                var element = _connectionIndexesProperty.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(element, new GUIContent($"Index {i}"));
                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    _connectionIndexesProperty.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();
                    return;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Connection Index"))
            {
                _connectionIndexesProperty.InsertArrayElementAtIndex(_connectionIndexesProperty.arraySize);
                var newElement = _connectionIndexesProperty.GetArrayElementAtIndex(_connectionIndexesProperty.arraySize - 1);
                newElement.intValue = 0;
            }

            EditorGUILayout.Space();

            // Conditions
            EditorGUILayout.LabelField("Conditions", EditorStyles.boldLabel);
            DrawScriptableObjectList(_conditionsProperty, "Conditions", target as TriggerStoneSO);

            if (GUILayout.Button("Add Condition"))
            {
                ShowSelectScriptableObjectMenu("Conditions", AddNewCondition);
            }

            EditorGUILayout.Space();

            // Event Listeners
            EditorGUILayout.LabelField("Event Listeners", EditorStyles.boldLabel);
            DrawScriptableObjectList(_eventListenersProperty, "Event Listeners", target as TriggerStoneSO);

            if (GUILayout.Button("Add Event Listener"))
            {
                ShowSelectScriptableObjectMenu("EventListeners", AddNewEventListener);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawScriptableObjectList(SerializedProperty listProperty, string label, TriggerStoneSO triggerSO)
        {
            for (int i = 0; i < listProperty.arraySize; i++)
            {
                SerializedProperty element = listProperty.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(element, new GUIContent($"{label} {i}"));

                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    var so = element.objectReferenceValue as ScriptableObject;
                    if (so != null)
                    {
                        Undo.RecordObject(so, "Remove SO");
                        AssetDatabase.RemoveObjectFromAsset(so);
                        DestroyImmediate(so, true);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(triggerSO));
                    }

                    listProperty.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();
                    return;
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private void ShowSelectScriptableObjectMenu(string category, Action<Type> callback)
        {
            var menu = new GenericMenu();
            var conditionBaseType = GetBaseTypeForCategory(category);
            if (conditionBaseType == null)
            {
                Debug.LogWarning($"No base type found for category {category}");
                return;
            }

            var derivedTypes = Assembly.GetAssembly(conditionBaseType)
                                       .GetTypes()
                                       .Where(t => t.IsSubclassOf(conditionBaseType) && !t.IsAbstract)
                                       .ToList();

            if (derivedTypes.Count == 0)
            {
                menu.AddDisabledItem(new GUIContent("No Types Available"));
            }
            else
            {
                foreach (var type in derivedTypes)
                {
                    var typeName = type.Name;
                    menu.AddItem(new GUIContent(typeName), false, () => callback(type));
                }
            }

            menu.ShowAsContext();
        }

        private Type GetBaseTypeForCategory(string category)
        {
            if (category == "Conditions")
                return typeof(ConditionSO);
            else if (category == "EventListeners")
                return typeof(EventListenerSOBase);

            return null;
        }

        private void AddNewCondition(Type type)
        {
            AddNewScriptableObject(_conditionsProperty, type);
        }

        private void AddNewEventListener(Type type)
        {
            AddNewScriptableObject(_eventListenersProperty, type);
        }

        private void AddNewScriptableObject(SerializedProperty listProperty, Type type)
        {
            var triggerSO = (TriggerStoneSO)target;
            var obj = CreateInstance(type);
            obj.name = type.Name;
            AssetDatabase.AddObjectToAsset(obj, triggerSO);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(triggerSO));

            listProperty.InsertArrayElementAtIndex(listProperty.arraySize);
            var newElement = listProperty.GetArrayElementAtIndex(listProperty.arraySize - 1);
            newElement.objectReferenceValue = obj;

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
