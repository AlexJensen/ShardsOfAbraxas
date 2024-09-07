using Abraxas.Cards.Data;
using Abraxas.Decks.Scriptable_Objects;
using Abraxas.Stones;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Abraxas.Editor
{
    public class DeckEditorWindow : EditorWindow
    {
        private ListView _deckListView;
        private TextField _deckTitleView;
        private List<DeckDataSO> _decks;
        private DeckDataSO _selectedDeck;
        private CardEditor _cardEditor;
        private VisualElement _deckDetailsContainer;

        [MenuItem("Window/Deck Editor")]
        public static void ShowExample()
        {
            DeckEditorWindow wnd = GetWindow<DeckEditorWindow>();
            wnd.titleContent = new GUIContent("Deck Editor");
        }

        public void CreateGUI()
        {
            var scrollView = new ScrollView();
            var container = new VisualElement();

            // Load and clone a visual tree asset into the root of this window.
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/AbraxasAssets/Editor/DeckEditorWindow/DeckEditorWindow.uxml");
            visualTree.CloneTree(container);

            // Find elements
            var createDeckButton = container.Q<Button>("create-deck-button");
            var createRandomDeckButton = container.Q<Button>("create-random-deck-button");
            _deckListView = container.Q<ListView>("deck-list-view");
            _deckTitleView = container.Q<TextField>("deck-title-field");
            createDeckButton.clicked += CreateNewDeck;
            createRandomDeckButton.clicked += CreateRandomDeck;

            // Initialize list views
            _decks = LoadAllDecks();
            _deckListView.makeItem = () => new Label();
            _deckListView.bindItem = (element, i) =>
            {
                var label = element as Label;
                label.text = _decks[i] != null ? _decks[i].name : "Missing Reference";
            };
            _deckListView.itemsSource = _decks;
            _deckListView.selectionChanged += OnDeckSelected;

            scrollView.Add(container);
            rootVisualElement.Add(scrollView);

            // Initialize the card editor
            _cardEditor = new CardEditor();
            _cardEditor.Initialize();

            // Add the card editor to the container
            _deckDetailsContainer = container.Q<VisualElement>("deck-details-container");
            _deckDetailsContainer.style.display = DisplayStyle.None;
            _deckDetailsContainer.Add(_cardEditor.GetRoot());
        }

        private List<DeckDataSO> LoadAllDecks()
        {
            var deckList = new List<DeckDataSO>();
            var guids = AssetDatabase.FindAssets("t:DeckDataSO", new[] { "Assets/AbraxasAssets/Resources/Decks" });

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var deck = AssetDatabase.LoadAssetAtPath<DeckDataSO>(path);
                if (deck != null)
                {
                    deckList.Add(deck);
                }
            }

            return deckList;
        }

        private void CreateNewDeck()
        {
            var newDeck = CreateInstance<DeckDataSO>();
            newDeck.name = GetUniqueDeckName("New Deck");
            var path = $"Assets/AbraxasAssets/Resources/Decks/{newDeck.name}.asset";
            AssetDatabase.CreateAsset(newDeck, path);
            AssetDatabase.SaveAssets();

            _decks.Add(newDeck);
            _deckListView.Rebuild();
        }

        private void CreateOneOfEachDeck()
        {
            var newDeck = CreateInstance<DeckDataSO>();
            newDeck.name = GetUniqueDeckName("OneOfEach");
            var path = $"Assets/AbraxasAssets/Resources/Decks/{newDeck.name}.asset";
            AssetDatabase.CreateAsset(newDeck, path);

            int currentStone = 0;
            for (int attack = 1; attack <= 10; attack++)
            {
                for (int defense = 1; defense <= 10; defense++)
                {
                    for (int speed = 2; speed <= 4; speed++)
                    {
                        for (int range = 0; range <= 0; range++)
                        {
                            var newCard = CreateInstance<CardDataSO>();

                            var data = newCard.Data;
                            data.StatBlock.Stats.ATK = attack;
                            data.StatBlock.Stats.DEF = defense;
                            data.StatBlock.Stats.SPD = speed;
                            data.StatBlock.Stats.RNG = range;
                            data.StatBlock.Cost = (attack *2) + (defense *2) + (speed * 4) + (range * 4) + 1;
                            data.StatBlock.StoneType = (StoneType)(currentStone++);
                            data.Title = $"Model ({attack}, {defense}, {speed}, {range})";
                            if (currentStone >= Enum.GetValues(typeof(StoneType)).Length / 6)
                            {
                                currentStone = 0;
                            }
                            newCard.Data = data;
                            newCard.name = data.Title;
                            AssetDatabase.AddObjectToAsset(newCard, newDeck);
                            newDeck.Cards.Add(newCard);
                        }
                    }
                }
            }

            AssetDatabase.SaveAssets();

            _decks.Add(newDeck);
            _deckListView.Rebuild();
        }

        private void CreateRandomDeck()
        {
            var newDeck = CreateInstance<DeckDataSO>();
            newDeck.name = GetUniqueDeckName("RandomDeck");
            var path = $"Assets/AbraxasAssets/Resources/Decks/{newDeck.name}.asset";
            AssetDatabase.CreateAsset(newDeck, path);

            // Select random stone colors (between 1 and 12)
            int numStoneColors = UnityEngine.Random.Range(1, 13);
            List<StoneType> selectedStones = Enum.GetValues(typeof(StoneType))
                                                  .Cast<StoneType>()
                                                  .OrderBy(x => UnityEngine.Random.value)
                                                  .Take(numStoneColors)
                                                  .ToList();

            for (int i = 1; i <= 12; i++)
            {
                // Generate random ATK and DEF values between 0 and 10
                int attack = UnityEngine.Random.Range(0, 11);
                int defense = UnityEngine.Random.Range(0, 11);

                // Generate random SPD between 0 and 4
                int speed = UnityEngine.Random.Range(0, 5);

                // Generate random RNG between 0 and 4
                int range = UnityEngine.Random.Range(0, 5);

                // Select a random stone from the chosen ones
                StoneType stoneType = selectedStones[UnityEngine.Random.Range(0, selectedStones.Count)];

                // Create the new card
                var newCard = CreateInstance<CardDataSO>();

                var data = newCard.Data;
                data.StatBlock.Stats.ATK = attack;
                data.StatBlock.Stats.DEF = defense;
                data.StatBlock.Stats.SPD = speed;
                data.StatBlock.Stats.RNG = range;
                data.StatBlock.Cost = (attack * 2) + (defense * 2) + (speed * 4) + (range * 8) + 1;
                data.StatBlock.StoneType = stoneType;
                data.Title = $"Model {ToRoman(i)}";
                newCard.Data = data;
                newCard.name = data.Title;

                AssetDatabase.AddObjectToAsset(newCard, newDeck);

                // Add 5 copies of each card to the deck
                for (int j = 0; j < 5; j++)
                {
                    newDeck.Cards.Add(newCard);
                }
            }

            AssetDatabase.SaveAssets();

            _decks.Add(newDeck);
            _deckListView.Rebuild();
        }

        // Helper method to convert integer to Roman numeral
        private string ToRoman(int number)
        {
            if ((number < 0) || (number > 3999)) return string.Empty;
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900);
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            return string.Empty;
        }

        private string GetUniqueDeckName(string baseName)
        {
            var path = $"Assets/AbraxasAssets/Resources/Decks/{baseName}.asset";
            int count = 1;

            while (File.Exists(path))
            {
                path = $"Assets/AbraxasAssets/Resources/Decks/{baseName} {count}.asset";
                count++;
            }

            return Path.GetFileNameWithoutExtension(path);
        }

        private void OnDeckSelected(IEnumerable<object> selectedItems)
        {
            _selectedDeck = (DeckDataSO)selectedItems.FirstOrDefault();
            DisplayDeckDetails();
            _cardEditor.SetDeck(_selectedDeck);
        }

        private void DisplayDeckDetails()
        {
            if (_selectedDeck == null)
                return;

            _deckDetailsContainer.style.display = DisplayStyle.Flex;

            _deckTitleView.value = _selectedDeck.name;

            _deckTitleView.RegisterValueChangedCallback(evt =>
            {
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(_selectedDeck), evt.newValue);
                _deckListView.Rebuild();
                EditorUtility.SetDirty(_selectedDeck);
                AssetDatabase.SaveAssets();
            });
        }
    }
}

