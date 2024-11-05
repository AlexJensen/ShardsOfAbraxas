using Abraxas.Stones.Conditions;
using Abraxas.Stones.Triggers;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

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
        BasicStoneData _data = new();
        #endregion

        #region Properties
        public List<ScriptableObject> Conditions { get => _conditions; set => _conditions = value; }
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

            // Serialize Data
            Data.NetworkSerialize(serializer);
        }
        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TriggerStoneSO))]
    public class TriggerStoneDataSOEditor : StoneSOEditor
    {
        private SerializedProperty _conditionsProperty;
        private SerializedProperty _dataProperty;
        private SerializedProperty _connectionIndexesProperty;

        private void OnEnable()
        {
            _conditionsProperty = serializedObject.FindProperty("_conditions");
            _dataProperty = serializedObject.FindProperty("_data");
            _connectionIndexesProperty = serializedObject.FindProperty("ConnectionIndexes");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_dataProperty);

            EditorGUILayout.LabelField("Conditions", EditorStyles.boldLabel);
            for (int i = 0; i < _conditionsProperty.arraySize; i++)
            {
                SerializedProperty conditionProperty = _conditionsProperty.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(conditionProperty);

                if (GUILayout.Button("Remove Condition"))
                {
                    var condition = conditionProperty.objectReferenceValue as ScriptableObject;
                    if (condition != null)
                    {
                        Undo.RecordObject(condition, "Remove Condition");
                        AssetDatabase.RemoveObjectFromAsset(condition);
                        DestroyImmediate(condition, true);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target));
                    }

                    _conditionsProperty.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();
                }
            }

            if (GUILayout.Button("Add Condition"))
            {
                ShowAddConditionMenu();
            }

            // Display ConnectionIndexes
            EditorGUILayout.LabelField("Connection Indexes", EditorStyles.boldLabel);
            for (int i = 0; i < _connectionIndexesProperty.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();

                SerializedProperty indexProperty = _connectionIndexesProperty.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(indexProperty, GUIContent.none);

                if (GUILayout.Button("Remove", GUILayout.Width(70)))
                {
                    _connectionIndexesProperty.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Connection Index"))
            {
                _connectionIndexesProperty.InsertArrayElementAtIndex(_connectionIndexesProperty.arraySize);
                _connectionIndexesProperty.GetArrayElementAtIndex(_connectionIndexesProperty.arraySize - 1).intValue = 0; // Default value
                serializedObject.ApplyModifiedProperties();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void ShowAddConditionMenu()
        {
            ShowSelectTypeMenu<ICondition>(type => AddCondition(type));
        }

        private void AddCondition(Type type)
        {
            var condition = CreateInstance(type);
            condition.name = type.Name;
            AssetDatabase.AddObjectToAsset(condition, target);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(condition));

            _conditionsProperty.InsertArrayElementAtIndex(_conditionsProperty.arraySize);
            _conditionsProperty.GetArrayElementAtIndex(_conditionsProperty.arraySize - 1).objectReferenceValue = condition;

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
