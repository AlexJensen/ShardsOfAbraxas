using Abraxas.Cards.Data;
using Abraxas.Decks.Scriptable_Objects;
using Abraxas.Stones;
using Abraxas.Stones.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DeckEditorWindow : EditorWindow
{
    private ListView _deckListView;
    private List<DeckDataSO> _decks;
    private DeckDataSO _selectedDeck;

    private ListView _cardListView;
    private List<CardDataSO> _cards;
    private CardDataSO _selectedCard;

    private ListView _stoneListView;
    private List<StoneDataSO> _stones;
    private StoneDataSO _selectedStone;

    private TextField _cardTitleField;
    private IntegerField _cardCostField, _cardAtkField, _cardDefField, _cardSpdField, _cardRngField;
    private EnumField _cardStoneTypeField;

    [MenuItem("Window/Deck Editor")]
    public static void ShowExample()
    {
        DeckEditorWindow wnd = GetWindow<DeckEditorWindow>();
        wnd.titleContent = new GUIContent("Deck Editor");
    }

    public void CreateGUI()
    {
        // Load and clone a visual tree asset into the root of this window.
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/AbraxasAssets/Editor/DeckEditorWindow/DeckEditorWindow.uxml");
        visualTree.CloneTree(rootVisualElement);

        // Add stylesheet
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/AbraxasAssets/Editor/DeckEditorWindow/DeckEditorWindow.uss");
        rootVisualElement.styleSheets.Add(styleSheet);

        // Find elements
        var createDeckButton = rootVisualElement.Q<Button>("create-deck-button");
        var addCardButton = rootVisualElement.Q<Button>("add-card-button");
        var addStoneButton = rootVisualElement.Q<Button>("add-stone-button");
        _deckListView = rootVisualElement.Q<ListView>("deck-list-view");
        _cardListView = rootVisualElement.Q<ListView>("card-list-view");
        _cardTitleField = rootVisualElement.Q<TextField>("card-title-field");
        _cardCostField = rootVisualElement.Q<IntegerField>("card-cost-field");
        _cardAtkField = rootVisualElement.Q<IntegerField>("card-atk-field");
        _cardDefField = rootVisualElement.Q<IntegerField>("card-def-field");
        _cardSpdField = rootVisualElement.Q<IntegerField>("card-spd-field");
        _cardRngField = rootVisualElement.Q<IntegerField>("card-rng-field");
        _cardStoneTypeField = rootVisualElement.Q<EnumField>("card-stone-type-field");
        _cardStoneTypeField.Init(StoneType.GARNET);
        _stoneListView = rootVisualElement.Q<ListView>("stone-list-view");

        // Set button click event
        createDeckButton.clicked += CreateNewDeck;
        addCardButton.clicked += AddNewCard;
        addStoneButton.clicked += ShowSelectStoneTypeMenu;

        // Initialize list views
        _decks = LoadAllDecks();
        _deckListView.makeItem = () => new Label();
        _deckListView.bindItem = (element, i) => (element as Label).text = _decks[i]?.name ?? "Missing Reference";
        _deckListView.itemsSource = _decks;
        _deckListView.selectionChanged += OnDeckSelected;

        _cards = new List<CardDataSO>();
        _cardListView.makeItem = () => new Label();
        _cardListView.bindItem = (element, i) => (element as Label).text = _cards[i]?.Data.Title ?? "Missing Reference";
        _cardListView.itemsSource = _cards;
        _cardListView.selectionChanged += OnCardSelected;

        _stones = new List<StoneDataSO>();
        _stoneListView.makeItem = () => new Label();
        _stoneListView.bindItem = (element, i) => (element as Label).text = _stones[i]?.name ?? "Missing Reference";
        _stoneListView.itemsSource = _stones;

        var cardListContainer = rootVisualElement.Q<VisualElement>("card-list-container");
        cardListContainer.style.display = DisplayStyle.None;

        var cardDetailContainer = rootVisualElement.Q<VisualElement>("card-details-container");
        cardDetailContainer.style.display = DisplayStyle.None;
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
        LoadDeckCards();
    }

    private void LoadDeckCards()
    {
        _cards.Clear();

        if (_selectedDeck != null)
        {
            foreach (var cardSO in _selectedDeck.Cards)
            {
                if (cardSO is CardDataSO cardDataSO)
                {
                    _cards.Add(cardDataSO);
                }
            }
        }

        _cardListView.Rebuild();

        // Show the card list container
        var cardListContainer = rootVisualElement.Q<VisualElement>("card-list-container");
        cardListContainer.style.display = DisplayStyle.Flex;

        // Hide the card detail container
        var cardDetailContainer = rootVisualElement.Q<VisualElement>("card-details-container");
        cardDetailContainer.style.display = DisplayStyle.None;
    }

    private void AddNewCard()
    {
        if (_selectedDeck == null)
            return;

        var newCard = CreateInstance<CardDataSO>();
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

    private void OnCardSelected(IEnumerable<object> selectedItems)
    {
        _selectedCard = (CardDataSO)selectedItems.FirstOrDefault();
        DisplayCardDetails();
    }

    private void DisplayCardDetails()
    {
        if (_selectedCard == null)
            return;

        _cardTitleField.value = _selectedCard.Data.Title;
        _cardCostField.value = _selectedCard.Data.StatBlock.Cost;
        _cardAtkField.value = _selectedCard.Data.StatBlock.Stats.ATK;
        _cardDefField.value = _selectedCard.Data.StatBlock.Stats.DEF;
        _cardSpdField.value = _selectedCard.Data.StatBlock.Stats.SPD;
        _cardRngField.value = _selectedCard.Data.StatBlock.Stats.RNG;
        _cardStoneTypeField.value = _selectedCard.Data.StatBlock.StoneType;

        // Show the card detail container
        var cardDetailContainer = rootVisualElement.Q<VisualElement>("card-details-container");
        cardDetailContainer.style.display = DisplayStyle.Flex;

        _cardTitleField.RegisterValueChangedCallback(evt =>
        {
            var data = _selectedCard.Data;
            data.Title = evt.newValue;
            _selectedCard.name = evt.newValue;
            _selectedCard.Data = data;
            EditorUtility.SetDirty(_selectedCard);
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
            var data = _selectedCard.Data;
            var statBlock = data.StatBlock;
            statBlock.Stats.ATK = evt.newValue;
            data.StatBlock = statBlock;
            _selectedCard.Data = data;
            EditorUtility.SetDirty(_selectedCard);
            AssetDatabase.SaveAssets();
        });

        _cardDefField.RegisterValueChangedCallback(evt =>
        {
            var data = _selectedCard.Data;
            var statBlock = data.StatBlock;
            statBlock.Stats.DEF = evt.newValue;
            data.StatBlock = statBlock;
            _selectedCard.Data = data;
            EditorUtility.SetDirty(_selectedCard);
            AssetDatabase.SaveAssets();
        });

        _cardSpdField.RegisterValueChangedCallback(evt =>
        {
            var data = _selectedCard.Data;
            var statBlock = data.StatBlock;
            statBlock.Stats.SPD = evt.newValue;
            data.StatBlock = statBlock;
            _selectedCard.Data = data;
            EditorUtility.SetDirty(_selectedCard);
            AssetDatabase.SaveAssets();
        });

        _cardRngField.RegisterValueChangedCallback(evt =>
        {
            var data = _selectedCard.Data;
            var statBlock = data.StatBlock;
            statBlock.Stats.RNG = evt.newValue;
            data.StatBlock = statBlock;
            _selectedCard.Data = data;
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


    private void ShowSelectStoneTypeMenu()
    {
        var menu = new GenericMenu();

        menu.AddItem(new GUIContent("Trigger Stone"), false, () => AddNewStone(typeof(TriggerStoneDataSO)));

        var effectTypes = Assembly.GetAssembly(typeof(EffectStoneDataSO))
                                  .GetTypes()
                                  .Where(t => t.IsSubclassOf(typeof(EffectStoneDataSO)) && !t.IsAbstract)
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
        if (_selectedCard == null)
            return;

        var stone = (StoneDataSO)CreateInstance(type);
        stone.name = type.Name;
        AssetDatabase.AddObjectToAsset(stone, _selectedCard);
        AssetDatabase.SaveAssets();
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(stone));

        var data = _selectedCard.Data;
        data.Stones.Add(new StoneWrapper { RuntimeStoneData = stone });
        _selectedCard.Data = data;
        EditorUtility.SetDirty(_selectedCard);
        AssetDatabase.SaveAssets();

        _stones.Add(stone);
        _stoneListView.Rebuild();
    }

}

