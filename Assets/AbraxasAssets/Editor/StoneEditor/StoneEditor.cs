using Abraxas.Cards.Data;
using Abraxas.StatBlocks.Data;
using Abraxas.Stones.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class StoneEditor
{
    private VisualElement _root;
    private ListView _stoneListView;
    private List<StoneConnector> _stones = new();
    private CardDataSO _selectedCard;
    private StoneConnector _selectedStone;
    private VisualElement _stoneDetailsContainer;
    private SerializedObject _serializedStone;
    private List<SerializedProperty> _stoneProperties;

    #region Dependencies
    public void Initialize()
    {
        _root = new VisualElement();

        // Load UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/AbraxasAssets/Editor/StoneEditor/StoneEditor.uxml");
        visualTree.CloneTree(_root);

        _root.Q<Button>("add-stone-button").clicked += AddNewStone;
        _root.Q<Button>("copy-stone-button").clicked += CopySelectedStone;

        _stoneListView = _root.Q<ListView>("stone-list-view");

        // Setup ListView
        _stoneListView.makeItem = () => new Label();
        _stoneListView.bindItem = (element, i) =>
        {
            var label = element as Label;
            label.text = _stones[i] != null ? _stones[i].RuntimeStoneData.name : "Missing Reference";
        };
        _stoneListView.itemsSource = _stones;
        _stoneListView.selectionChanged += OnStoneSelected;

        _stoneDetailsContainer = _root.Q<VisualElement>("stone-details-container");
        _stoneDetailsContainer.style.display = DisplayStyle.None;
    }
    #endregion

    #region Methods
    public VisualElement GetRoot() => _root;

    public void SetCard(CardDataSO card)
    {
        _selectedCard = card;
        if (_selectedCard != null && _selectedCard.Data.Stones != null)
        {
            _stones = _selectedCard.Data.Stones;
        }
        else
        {
            _stones = new List<StoneConnector>();
        }
        _stoneListView.itemsSource = _stones;
        _stoneDetailsContainer.style.display = DisplayStyle.None;
        _stoneListView.Rebuild();
    }

    private void OnStoneSelected(IEnumerable<object> selectedItems)
    {
        _selectedStone = (StoneConnector)selectedItems.FirstOrDefault();
        if (_selectedStone?.RuntimeStoneData == null)
        {
            _stoneDetailsContainer.style.display = DisplayStyle.None;
            return;
        }

        _serializedStone = new SerializedObject(_selectedStone.RuntimeStoneData);
        var dataProperty = _serializedStone.FindProperty("_data");
        if (dataProperty == null)
        {
            _stoneDetailsContainer.style.display = DisplayStyle.None;
            return;
        }

        _stoneProperties = dataProperty.Copy().GetChildren().ToList();
        DisplayStoneDetails();
        _stoneDetailsContainer.style.display = _selectedStone != null ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void DisplayStoneDetails()
    {
        _stoneDetailsContainer.Clear();

        if (_selectedStone == null)
        {
            _stoneDetailsContainer.style.display = DisplayStyle.None;
            return;
        }

        // Add "Stone Details" label
        var stoneDetailsLabel = new Label("Stone Details");
        stoneDetailsLabel.AddToClassList("label");
        _stoneDetailsContainer.Add(stoneDetailsLabel);

        foreach (var property in _stoneProperties)
        {
            // Create a container for each field
            var fieldContainer = new VisualElement();
            fieldContainer.AddToClassList("field-container");
            _stoneDetailsContainer.Add(fieldContainer);

            // Handle StatData separately
            if (property.propertyType == SerializedPropertyType.Generic && property.type == nameof(StatData))
            {
                var fieldLabel = new Label($"{property.displayName} ");
                fieldLabel.AddToClassList("label");
                fieldContainer.Add(fieldLabel);

                var statDataProperty = property.Copy();
                statDataProperty.NextVisible(true); // Move inside the StatData object

                while (statDataProperty.propertyPath.StartsWith(property.propertyPath))
                {
                    var statField = new PropertyField(statDataProperty);
                    statField.Bind(_serializedStone);
                    fieldContainer.Add(statField);
                    statDataProperty.NextVisible(false);
                }
            }
            else
            {
                var field = new PropertyField(property);
                field.Bind(_serializedStone);
                fieldContainer.Add(field);
            }
        }

        _stoneDetailsContainer.style.display = DisplayStyle.Flex;
    }

    private void AddNewStone()
    {
        var menu = new GenericMenu();

        var stoneTypes = Assembly.GetAssembly(typeof(StoneSO))
                                 .GetTypes()
                                 .Where(t => t.IsSubclassOf(typeof(StoneSO)) && !t.IsAbstract)
                                 .ToList();

        foreach (var stoneType in stoneTypes)
        {
            menu.AddItem(new GUIContent(stoneType.Name), false, () => CreateNewStone(stoneType));
        }

        menu.ShowAsContext();
    }

    private void CreateNewStone(Type stoneType)
    {
        var newStone = ScriptableObject.CreateInstance(stoneType) as StoneSO;
        newStone.name = stoneType.Name;

        AssetDatabase.AddObjectToAsset(newStone, _selectedCard);
        EditorUtility.SetDirty(_selectedCard);
        AssetDatabase.SaveAssets();
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newStone));

        if (_selectedCard.Data.Stones == null)
        {
            var data = _selectedCard.Data;
            data.Stones = new List<StoneConnector>();
            _selectedCard.Data = data;
        }
        var stoneConnector = new StoneConnector { RuntimeStoneData = newStone };
        _selectedCard.Data.Stones.Add(stoneConnector);
        _stones.Add(stoneConnector);
        _stoneListView.Rebuild();
    }

    private void CopySelectedStone()
    {
        if (_selectedStone == null) return;
        var copiedStone = UnityEngine.Object.Instantiate(_selectedStone.RuntimeStoneData);
        AssetDatabase.AddObjectToAsset(copiedStone, _selectedCard);
        AssetDatabase.SaveAssets();
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(copiedStone));
        if (_selectedCard.Data.Stones == null)
        {
            var data = _selectedCard.Data;
            data.Stones = new List<StoneConnector>();
            _selectedCard.Data = data;
        }
        var stoneConnector = new StoneConnector { RuntimeStoneData = copiedStone };
        _selectedCard.Data.Stones.Add(stoneConnector);
        _stones.Add(stoneConnector);
        _stoneListView.Rebuild();
    }
    #endregion
}

public static class SerializedPropertyExtensions
{
    public static List<SerializedProperty> GetChildren(this SerializedProperty property)
    {
        var children = new List<SerializedProperty>();
        var iterator = property.Copy();
        if (!iterator.NextVisible(true)) return children;
        var end = property.GetEndProperty();
        do
        {
            if (SerializedProperty.EqualContents(iterator, end)) break;
            children.Add(iterator.Copy());
        } while (iterator.NextVisible(false));
        return children;
    }
}
