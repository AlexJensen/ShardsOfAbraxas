using Abraxas.Passives;
using Abraxas.Stones.Data;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Abraxas.Cards.Data
{
    /// <summary>
    /// CardDataSO is a ScriptableObject that contains a CardData struct used to display editable card data in the Unity inspector.
    /// </summary>
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
            private ScriptableObject existingStone;

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


                if (GUILayout.Button("Calculate Cost"))
                {
                    CalculateStatBlockCost();
                }

                EditorGUILayout.LabelField("Stones", EditorStyles.boldLabel);
                for (int i = 0; i < _stonesProperty.arraySize; i++)
                {
                    SerializedProperty stoneProperty = _stonesProperty.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(stoneProperty);

                    if (GUILayout.Button("Remove Stone"))
                    {
                        RemoveStone(i, stoneProperty);
                    }

                }

                // Add Stone Options
                if (GUILayout.Button("Add Stone"))
                {
                    ShowSelectStoneTypeMenu();
                }

                // Assign Existing Stone Section
                EditorGUILayout.LabelField("Assign Existing Stone", EditorStyles.boldLabel);

                // Persistent reference for the existing stone
                existingStone = EditorGUILayout.ObjectField(
                    "Existing Stone",
                    existingStone,
                    typeof(ScriptableObject),
                    false) as ScriptableObject;

                // Display button only if a valid ScriptableObject is assigned
                if (existingStone != null && (existingStone is TriggerStoneSO || existingStone is EffectStoneSO || existingStone is PassiveStoneSO))
                {

                    AssignExistingStone(existingStone);
                    existingStone = null;

                }

                serializedObject.ApplyModifiedProperties();
            }

            private void CalculateStatBlockCost()
            {
                var cardDataSO = (CardDataSO)target;
                var data = cardDataSO.Data;
                var statBlock = data.StatBlock;

                // Apply the cost formula
                statBlock.Cost = (statBlock.Stats.ATK * 2) + (statBlock.Stats.DEF * 2) +
                                 (statBlock.Stats.SPD * 4) + (statBlock.Stats.RNG * 8) + 1;
                data.StatBlock = statBlock;
                cardDataSO.Data = data;

                // Mark the object as dirty to ensure Unity saves the change
                EditorUtility.SetDirty(cardDataSO);

                Debug.Log($"Calculated Cost: {statBlock.Cost}");
            }

            private void RemoveStone(int i, SerializedProperty stoneProperty)
            {
                var stone = stoneProperty.objectReferenceValue as ScriptableObject;

                if (stone != null)
                {
                    if (AssetDatabase.IsMainAsset(stone) || AssetDatabase.IsSubAsset(stone))
                    {
                        // Only destroy the stone if it's part of the CardDataSO asset
                        if (AssetDatabase.GetAssetPath(stone) == AssetDatabase.GetAssetPath(target))
                        {
                            Undo.RecordObject(stone, "Remove Stone");
                            AssetDatabase.RemoveObjectFromAsset(stone);
                            DestroyImmediate(stone, true);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target));
                        }
                    }
                }

                _stonesProperty.DeleteArrayElementAtIndex(i); // Remove the reference from the array
                serializedObject.ApplyModifiedProperties();
            }

            private void AssignExistingStone(ScriptableObject existingStone)
            {
                // Ensure the object is a valid Stone type
                if (!(existingStone is TriggerStoneSO || existingStone is EffectStoneSO || existingStone is PassiveStoneSO))
                {
                    Debug.LogWarning("Assigned object is not a valid Stone type.");
                    return;
                }

                // Add the existing stone to the array
                _stonesProperty.arraySize++; // Increase array size
                var newElement = _stonesProperty.GetArrayElementAtIndex(_stonesProperty.arraySize - 1); // Get new element
                newElement.objectReferenceValue = existingStone; // Assign the ScriptableObject

                // Apply changes
                serializedObject.ApplyModifiedProperties();

                Debug.Log($"Assigned existing Stone: {existingStone.name}");
            }

            private void ShowSelectStoneTypeMenu()
            {
                var menu = new GenericMenu();

                menu.AddItem(new GUIContent("Trigger Stone"), false, () => AddNewStone(typeof(TriggerStoneSO)));

                var effectTypes = Assembly.GetAssembly(typeof(EffectStoneSO))
                                            .GetTypes()
                                            .Where(t => t.IsSubclassOf(typeof(EffectStoneSO)) && !t.IsAbstract)
                                            .ToList();

                foreach (var effectType in effectTypes)
                {
                    var effectTypeName = effectType.Name.Replace("Effect", "").Replace("DataSO", "");
                    menu.AddItem(new GUIContent($"Effect Stones/{effectTypeName}"), false, () => AddNewStone(effectType));
                }
                var passiveTypes = Assembly.GetAssembly(typeof(PassiveStoneSO))
                                            .GetTypes()
                                            .Where(t => t.IsSubclassOf(typeof(PassiveStoneSO)) && !t.IsAbstract)
                                            .ToList();

                foreach (var passiveType in passiveTypes)
                {
                    var passiveTypeName = passiveType.Name.Replace("Passive", "").Replace("DataSO", "");
                    menu.AddItem(new GUIContent($"Passive Stones/{passiveTypeName}"), false, () => AddNewStone(passiveType));
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
                newElement.objectReferenceValue = stone;

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}