using Abraxas.Cards.Data;
using System.Collections.Generic;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditorInternal;
#endif
using UnityEngine;

namespace Abraxas.Decks.Scriptable_Objects
{
    [CreateAssetMenu(fileName = "New Deck", menuName = "Abraxas/Data/Deck")]
    public class DeckDataSO : ScriptableObject
    {
        #region Fields
        [SerializeField]
        List<ScriptableObject> _cards = new();
        #endregion

        #region Properties
        public List<ScriptableObject> Cards { get => _cards; set => _cards = value; }
        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(DeckDataSO))]
    public class DeckDataSOEditor : Editor
    {
        private SerializedProperty _cardsProperty;
        private ReorderableList _cardsReorderableList;

        private void OnEnable()
        {
            _cardsProperty = serializedObject.FindProperty("_cards");

            _cardsReorderableList = new ReorderableList(serializedObject, _cardsProperty, true, true, true, true)
            {
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Cards"),

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var element = _cardsProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
                },

                onAddCallback = list =>
                {
                    var card = CreateInstance<CardDataSO>();
                    card.name = "New Card";
                    AssetDatabase.AddObjectToAsset(card, target);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(card));

                    _cardsProperty.InsertArrayElementAtIndex(_cardsProperty.arraySize);
                    _cardsProperty.GetArrayElementAtIndex(_cardsProperty.arraySize - 1).objectReferenceValue = card;

                    serializedObject.ApplyModifiedProperties();
                },

                onRemoveCallback = list =>
                {
                    var element = _cardsProperty.GetArrayElementAtIndex(list.index);
                    var card = element.objectReferenceValue as ScriptableObject;
                    if (card != null)
                    {
                        Undo.RecordObject(card, "Remove Card");
                        AssetDatabase.RemoveObjectFromAsset(card);
                        DestroyImmediate(card, true);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target));
                    }

                    ReorderableList.defaultBehaviours.DoRemoveButton(list);
                    serializedObject.ApplyModifiedProperties();
                }
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _cardsReorderableList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
    #endif
}
