using Abraxas.Stones.Data;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Abraxas.Cards.Data
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Abraxas/Data/Card")]
    public class CardDataSO : ScriptableObject
    {
        #region Fields
        [SerializeField]
        CardData _data = new();
        #endregion

        #region Properties
        public CardData Data
        {
            get => _data;
            set => _data = value;
        }
        #endregion

#if UNITY_EDITOR
        [CustomEditor(typeof(CardDataSO))]
        public class CardDataSOEditor : Editor
        {
            private SerializedProperty _dataProperty;
            private SerializedProperty _stonesProperty;

            private void OnEnable()
            {
                _dataProperty = serializedObject.FindProperty("_data");
                _stonesProperty = _dataProperty.FindPropertyRelative("Stones");
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                EditorGUILayout.PropertyField(_dataProperty.FindPropertyRelative("Title"), new GUIContent("Title"));
                EditorGUILayout.PropertyField(_dataProperty.FindPropertyRelative("StatBlock"), new GUIContent("Stat Block"));

                EditorGUILayout.LabelField("Stones", EditorStyles.boldLabel);
                for (int i = 0; i < _stonesProperty.arraySize; i++)
                {
                    SerializedProperty stoneProperty = _stonesProperty.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(stoneProperty);

                    if (GUILayout.Button("Remove Stone"))
                    {
                        var stone = stoneProperty.FindPropertyRelative("RuntimeStoneData").objectReferenceValue as ScriptableObject;
                        if (stone != null)
                        {
                            Undo.RecordObject(stone, "Remove Stone");
                            AssetDatabase.RemoveObjectFromAsset(stone);
                            DestroyImmediate(stone, true);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target));
                        }

                        _stonesProperty.DeleteArrayElementAtIndex(i);
                        serializedObject.ApplyModifiedProperties();
                    }
                }

                if (GUILayout.Button("Add Stone"))
                {
                    ShowSelectStoneTypeMenu();
                }

                serializedObject.ApplyModifiedProperties();
            }

            private void ShowSelectStoneTypeMenu()
            {
                var menu = new GenericMenu();

                menu.AddItem(new GUIContent("Trigger Stone"), false, () => AddNewStone(typeof(TriggerSO)));

                var effectTypes = Assembly.GetAssembly(typeof(EffectSO))
                                            .GetTypes()
                                            .Where(t => t.IsSubclassOf(typeof(EffectSO)) && !t.IsAbstract)
                                            .ToList();

                foreach (var effectType in effectTypes)
                {
                    var effectTypeName = effectType.Name.Replace("Effect", "").Replace("DataSO", "");
                    menu.AddItem(new GUIContent($"Effect Stones/{effectTypeName}"), false, () => AddNewStone(effectType));
                }
                menu.ShowAsContext();
            }

            private void AddNewStone(Type type)
            {
                var card = (CardDataSO)target;
                var stone = CreateInstance(type);
                stone.name = type.Name;
                AssetDatabase.AddObjectToAsset(stone, card);
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(stone));

                _stonesProperty.InsertArrayElementAtIndex(_stonesProperty.arraySize);
                var newElement = _stonesProperty.GetArrayElementAtIndex(_stonesProperty.arraySize - 1);
                newElement.FindPropertyRelative("RuntimeStoneData").objectReferenceValue = stone;

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}