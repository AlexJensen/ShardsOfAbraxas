using Abraxas.Stones.Conditions;
using Abraxas.Stones.Triggers;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Abraxas.Stones.Data
{

    [CreateAssetMenu(fileName = "New Trigger", menuName = "Abraxas/StoneData/Trigger")]
    public class TriggerStoneSO : StoneSO
    {
        #region Fields
        [HideInInspector]
        public List<int> Indexes;

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
    }

    #if UNITY_EDITOR
        [CustomEditor(typeof(TriggerStoneSO))]
        public class TriggerStoneDataSOEditor : StoneSOEditor
        {
            private SerializedProperty _conditionsProperty;
            private SerializedProperty _dataProperty;

            private void OnEnable()
            {
                _conditionsProperty = serializedObject.FindProperty("_conditions");
                _dataProperty = serializedObject.FindProperty("_data");
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





