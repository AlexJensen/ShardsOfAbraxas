
using Abraxas.Stones.Controllers.StoneTypes.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Abraxas.Stones.Data
{

    [CreateAssetMenu(fileName = "New Trigger", menuName = "Abraxas/StoneData/Trigger")]
    public class TriggerStoneDataSO : StoneDataSO
    {
        [HideInInspector]
        public List<int> Indexes;

        [SerializeField]
        List<ScriptableObject> _conditions = new();

        [SerializeField]
        BasicStoneData _data = new();

        public List<ScriptableObject> Conditions { get => _conditions; set => _conditions = value; }
        public override Type ControllerType { get; set; } = typeof(TriggerStone);
        public override IStoneData Data { get => _data; set => _data = (BasicStoneData)value; }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(TriggerStoneDataSO))]
    public class TriggerStoneDataSOEditor : Editor
    {
        SerializedProperty conditionsProperty;
        SerializedProperty dataProperty;

        private void OnEnable()
        {
            conditionsProperty = serializedObject.FindProperty("_conditions");
            dataProperty = serializedObject.FindProperty("_data");
        }

        public override void OnInspectorGUI()
        {
            var triggerStoneDataSO = (TriggerStoneDataSO)target;

            serializedObject.Update();
            EditorGUILayout.PropertyField(dataProperty);

            EditorGUILayout.LabelField("Conditions", EditorStyles.boldLabel);
            for (int i = 0; i < conditionsProperty.arraySize; i++)
            {
                SerializedProperty conditionProperty = conditionsProperty.GetArrayElementAtIndex(i);
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

                    conditionsProperty.DeleteArrayElementAtIndex(i);
                    conditionsProperty.DeleteArrayElementAtIndex(i);

                    serializedObject.ApplyModifiedProperties();
                }
            }

            if (GUILayout.Button("Add Condition"))
            {
                GenericMenu menu = new();

                var conditionTypes = Assembly.GetAssembly(typeof(ICondition<>))
                                             .GetTypes()
                                             .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(ScriptableObject)) &&
                                                         t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICondition<>)))
                                             .ToList();

                foreach (var type in conditionTypes)
                {
                    menu.AddItem(new GUIContent(type.Name), false, () => AddCondition(type));
                }
                menu.ShowAsContext();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void AddCondition(Type type)
        {
            var condition = CreateInstance(type);
            condition.name = type.Name;

            AssetDatabase.AddObjectToAsset(condition, target);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(condition));

            conditionsProperty.InsertArrayElementAtIndex(conditionsProperty.arraySize);
            conditionsProperty.GetArrayElementAtIndex(conditionsProperty.arraySize - 1).objectReferenceValue = condition;

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
