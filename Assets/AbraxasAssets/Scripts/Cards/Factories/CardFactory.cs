using Abraxas.Cards.Controllers;
using Abraxas.Cards.Data;
using Abraxas.Cards.Managers;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.StackBlocks.Views;
using Abraxas.StatBlocks.Controllers;
using Abraxas.Stones.Controllers;
using Abraxas.Stones.Data;
using Abraxas.Stones.Triggers;
using System.Collections.Generic;
using Zenject;

namespace Abraxas.Cards.Factories
{
    class CardFactory : IFactory<CardData, ICardController>
    {
        #region Dependencies
        readonly DiContainer _container;
        readonly Card.Settings _cardSettings;
        readonly ICardManager _cardManager;
        readonly StoneController.Factory _stoneFactory;
        readonly StatBlockController.Factory _statBlockFactory;

        public CardFactory(
            DiContainer container,
            Card.Settings cardSettings,
            ICardManager cardManager,
            StoneController.Factory stoneFactory,
            StatBlockController.Factory statBlockFactory)
        {
            _container = container;
            _cardSettings = cardSettings;
            _cardManager = cardManager;
            _stoneFactory = stoneFactory;
            _statBlockFactory = statBlockFactory;
        }
        #endregion

        #region Methods
        public ICardController Create(CardData data)
        {
            var gameObject = _container.InstantiatePrefab(_cardSettings.cardPrefab);
            var view = gameObject.GetComponent<CardView>();
            var dragListener = gameObject.GetComponent<CardDragListener>();
            var mouseOverListener = gameObject.GetComponent<CardMouseOverListener>();
            var controller = _container.Instantiate<CardController>();
            var model = _container.Instantiate<CardModel>();
            var dragHandler = _container.Instantiate<CardDragHandler>();
            var mouseOverHandler = _container.Instantiate<CardMouseOverHandler>();

            var statBlockView = gameObject.GetComponent<StatBlockView>();
            var statBlockController = _statBlockFactory.Create(data.StatBlock, statBlockView);

            List<IStoneController> stoneControllers = new();
            Dictionary<StoneSO, IStoneController> stoneSoToControllerMap = new();

            // Initialize model, controller, and view
            model.Initialize(data, statBlockController, stoneControllers);
            controller.Initialize(model, view);
            view.Initialize(model, controller);

            // Create stone controllers and build the mapping
            if (data.Stones != null)
            {
                for (int i = 0; i < data.Stones.Count; i++)
                {
                    var stoneData = data.Stones[i];
                    var stoneController = _stoneFactory.Create(stoneData, controller);
                    stoneController.Index = i;

                    stoneControllers.Add(stoneController);

                    // Map StoneSO to its controller
                    stoneSoToControllerMap[stoneData] = stoneController;
                }
            }

            // Set up connections for trigger stones
            foreach (var stoneController in stoneControllers)
            {
                if (stoneController is TriggerStone triggerStoneController)
                {
                    var triggerStoneData = data.Stones[triggerStoneController.Index] as TriggerStoneSO;
                    if (triggerStoneData != null)
                    {
                        List<EffectStone> effects = new();

                        foreach (int index in triggerStoneData.ConnectionIndexes)
                        {
                            if (index >= 0 && index < data.Stones.Count)
                            {
                                var effectStoneData = data.Stones[index];
                                if (stoneSoToControllerMap.TryGetValue(effectStoneData, out var effectStoneController))
                                {
                                    if (effectStoneController is EffectStone effectStone)
                                    {
                                        effects.Add(effectStone);
                                    }
                                }
                            }
                        }

                        triggerStoneController.Effects = effects;
                    }
                }
            }

            // Initialize drag and mouse over handlers
            dragHandler.Initialize(dragListener, controller);
            dragListener.Initialize(dragHandler);
            mouseOverHandler.Initialize(controller);
            mouseOverListener.Initialize(mouseOverHandler);

            _cardManager.AddCard(controller);
            return controller;
        }
        #endregion
    }
}
