using Abraxas.Cards.Data;
using Abraxas.Decks.Scriptable_Objects;
using Abraxas.StatBlocks.Data;
using Abraxas.Stones;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Abraxas.Editor
{
    public class CardEditor
    {
        #region Fields
        private VisualElement _root;
        private ListView _cardListView;
        private List<CardDataSO> _cards;
        private DeckDataSO _selectedDeck;
        private CardDataSO _selectedCard;

        private TextField _cardTitleField;
        private IntegerField _cardCostField, _cardAtkField, _cardDefField, _cardSpdField, _cardRngField;
        private EnumField _cardStoneTypeField;
        private StoneEditor _stoneEditor;
        private VisualElement _cardDetailsContainer;


        #endregion

        #region Dependencies
        public void Initialize()
        {
            _root = new VisualElement();

            // Load and clone a visual tree asset into the root of this window.
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/AbraxasAssets/Editor/CardEditor/CardEditor.uxml");
            visualTree.CloneTree(_root);

           _root.Q<Button>("create-card-button").clicked += AddNewCard;
            _root.Q<Button>("copy-card-button").clicked += CopySelectedCard;

            _cardListView = _root.Q<ListView>("card-list-view");
            _cardTitleField = _root.Q<TextField>("card-title-field");
            _cardCostField = _root.Q<IntegerField>("card-cost-field");
            _cardAtkField = _root.Q<IntegerField>("card-atk-field");
            _cardDefField = _root.Q<IntegerField>("card-def-field");
            _cardSpdField = _root.Q<IntegerField>("card-spd-field");
            _cardRngField = _root.Q<IntegerField>("card-rng-field");
            _cardStoneTypeField = _root.Q<EnumField>("card-stone-type-field");
            _cardStoneTypeField.Init(StoneType.GARNET);

            _cards = new List<CardDataSO>();
            _cardListView.makeItem = () => new Label();
            _cardListView.bindItem = (element, i) =>
            {
                var label = element as Label;
                label.text = _cards[i] != null ? _cards[i].name : "Missing Reference";
            };
            _cardListView.itemsSource = _cards;
            _cardListView.selectionChanged += OnCardSelected;

            _stoneEditor = new StoneEditor();
            _stoneEditor.Initialize();
            _root.Q<VisualElement>("stone-editor-container").Add(_stoneEditor.GetRoot());

            _cardDetailsContainer = _root.Q<VisualElement>("card-details-container"); 

            // Hide the card details container initially
            _cardDetailsContainer.style.display = DisplayStyle.None;
        }
        #endregion

        #region Methods
        public VisualElement GetRoot()
        {
            return _root;
        }

        public void SetDeck(DeckDataSO deck)
        {
            _selectedDeck = deck;
            _cards = new List<CardDataSO>();

            if (deck != null)
            {
                foreach (var cardSO in deck.Cards)
                {
                    if (cardSO is CardDataSO cardDataSO)
                    {
                        _cards.Add(cardDataSO);
                    }
                }
            }
            _cardDetailsContainer.style.display = DisplayStyle.None;

            _cardListView.itemsSource = _cards;
            _cardListView.Rebuild();
        }

        private void AddNewCard()
        {
            if (_selectedDeck == null)
                return;

            var newCard = ScriptableObject.CreateInstance<CardDataSO>();
            newCard.name = "New Card";

            var cardData = newCard.Data;
            cardData.Title = newCard.name;
            newCard.Data = cardData;

            AssetDatabase.AddObjectToAsset(newCard, _selectedDeck);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newCard));

            _selectedDeck.Cards.Add(newCard);
            _cards.Add(newCard);

            _cardListView.Rebuild();
        }

        private void CopySelectedCard()
        {
            if (_selectedCard == null)
                return;

            // Create a copy of the selected card
            var cardCopy = UnityEngine.Object.Instantiate(_selectedCard);
            cardCopy.name = $"{_selectedCard.name} Copy";
            AssetDatabase.AddObjectToAsset(cardCopy, AssetDatabase.GetAssetPath(_selectedDeck));
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(cardCopy));

            // Add the copy to the deck and update the UI
            _selectedDeck.Cards.Add(cardCopy);
            _cards.Add(cardCopy);
            _cardListView.Rebuild();
        }

        private void OnCardSelected(IEnumerable<object> selectedItems)
        {
            _selectedCard = (CardDataSO)selectedItems.FirstOrDefault();
            DisplayCardDetails();
            _stoneEditor.SetCard(_selectedCard);
        }

        private int CalculateDefaultStatCost(CardDataSO card)
        {
            var calculatedCost = 1 +
                (card.Data.StatBlock.Stats.ATK * 2) +
                (card.Data.StatBlock.Stats.DEF * 2) +
                (card.Data.StatBlock.Stats.SPD * 3) +
                (card.Data.StatBlock.Stats.RNG * 4);

            _cardCostField.value = calculatedCost;
            return calculatedCost;
        }

        private void UpdateCardCost(StatBlockData statBlock)
        {
            statBlock.Cost = CalculateDefaultStatCost(_selectedCard);
            _cardCostField.value = statBlock.Cost;
            _cardCostField.MarkDirtyRepaint();
        }

        private void DisplayCardDetails()
        {
            if (_selectedCard == null)
            {
                // Hide the card details container if no card is selected
                _cardDetailsContainer.style.display = DisplayStyle.None;
                return;
            }

            _cardTitleField.value = _selectedCard.Data.Title;
            _cardCostField.value = _selectedCard.Data.StatBlock.Cost;
            _cardAtkField.value = _selectedCard.Data.StatBlock.Stats.ATK;
            _cardDefField.value = _selectedCard.Data.StatBlock.Stats.DEF;
            _cardSpdField.value = _selectedCard.Data.StatBlock.Stats.SPD;
            _cardRngField.value = _selectedCard.Data.StatBlock.Stats.RNG;
            _cardStoneTypeField.value = _selectedCard.Data.StatBlock.StoneType;

            // Show the card detail container
            _cardDetailsContainer.style.display = DisplayStyle.Flex;

            _cardTitleField.RegisterValueChangedCallback(evt =>
            {
                var data = _selectedCard.Data;
                data.Title = evt.newValue;
                _selectedCard.name = evt.newValue;
                _selectedCard.Data = data;
                EditorUtility.SetDirty(_selectedCard);
                _cardListView.Rebuild();
                AssetDatabase.SaveAssets();
            });

            _cardCostField.RegisterValueChangedCallback(evt =>
            {
                var data = _selectedCard.Data;
                var statBlock = data.StatBlock;
                statBlock.Cost = evt.newValue;
                data.StatBlock = statBlock;
                _selectedCard.Data = data;
                EditorUtility.SetDirty(_selectedCard);
                AssetDatabase.SaveAssets();
            });

            _cardAtkField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == _selectedCard.Data.StatBlock.Stats.ATK)
                    return;
                var data = _selectedCard.Data;
                var statBlock = data.StatBlock;
                statBlock.Stats.ATK = evt.newValue;
                data.StatBlock = statBlock;
                _selectedCard.Data = data;
                UpdateCardCost(statBlock);
                EditorUtility.SetDirty(_selectedCard);
                AssetDatabase.SaveAssets();
            });

            _cardDefField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == _selectedCard.Data.StatBlock.Stats.DEF)
                    return;
                var data = _selectedCard.Data;
                var statBlock = data.StatBlock;
                statBlock.Stats.DEF = evt.newValue;
                data.StatBlock = statBlock;
                _selectedCard.Data = data;
                UpdateCardCost(statBlock);
                EditorUtility.SetDirty(_selectedCard);
                AssetDatabase.SaveAssets();
            });

            _cardSpdField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == _selectedCard.Data.StatBlock.Stats.SPD)
                    return;
                var data = _selectedCard.Data;
                var statBlock = data.StatBlock;
                statBlock.Stats.SPD = evt.newValue;
                data.StatBlock = statBlock;
                _selectedCard.Data = data;
                UpdateCardCost(statBlock);
                EditorUtility.SetDirty(_selectedCard);
                AssetDatabase.SaveAssets();
            });

            _cardRngField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == _selectedCard.Data.StatBlock.Stats.RNG)
                    return;
                var data = _selectedCard.Data;
                var statBlock = data.StatBlock;
                statBlock.Stats.RNG = evt.newValue;
                data.StatBlock = statBlock;
                _selectedCard.Data = data;
                UpdateCardCost(statBlock);
                EditorUtility.SetDirty(_selectedCard);
                AssetDatabase.SaveAssets();
            });

            _cardStoneTypeField.RegisterValueChangedCallback(evt =>
            {
                if (_selectedCard != null)
                {
                    var data = _selectedCard.Data;
                    var statBlock = data.StatBlock;
                    statBlock.StoneType = (StoneType)evt.newValue;
                    data.StatBlock = statBlock;
                    _selectedCard.Data = data;
                    EditorUtility.SetDirty(_selectedCard);
                    AssetDatabase.SaveAssets();
                }
            });
        }
        #endregion
    }
}

