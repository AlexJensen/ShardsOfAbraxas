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
using System.Linq;
using Zenject;


namespace Abraxas.Cards.Factories
{
    /// <summary>
    /// CardFactory is a factory class for creating cards.
    /// </summary>
    class CardFactory : IFactory<CardData, ICardController>
    {
        #region Dependencies
        readonly DiContainer _container;
        readonly Card.Settings _cardSettings;
        readonly ICardManager _cardManager;
        readonly StoneController.Factory _stoneFactory;
        readonly StatBlockController.Factory _statBlockFactory;
        public CardFactory(DiContainer container, Card.Settings cardSettings, ICardManager cardManager, StoneController.Factory stoneFactory, StatBlockController.Factory statBlockFactory)
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
            if (data.Stones != null)
            {
                foreach (var stoneController in from stoneData in data.Stones
                                                let stoneController = _stoneFactory.Create(stoneData.RuntimeStoneData)
                                                select stoneController)
                {
                    stoneController.Card = controller;
                    stoneControllers.Add(stoneController);
                    stoneController.Index = stoneControllers.IndexOf(stoneController);
                }
            }

            model.Initialize(data, statBlockController, stoneControllers);
            controller.Initialize(model, view);
            view.Initialize(model, controller);

            foreach (var (triggerStoneController, stoneData) in
                from TriggerStone triggerStoneController
                in stoneControllers.OfType<TriggerStone>()
                let validIndexes = new List<int>()
                from stoneData in
                    from stoneData in data.Stones
                    where stoneData.Index == triggerStoneController.Index
                    let triggerStoneData = stoneData.RuntimeStoneData as TriggerStoneSO
                    where triggerStoneData != null
                    select stoneData
                select (triggerStoneController, stoneData))
            {
                triggerStoneController.Effects.AddRange(
                from index in stoneData.ConnectionIndexes
                where index >= 0 && index < stoneControllers.Count
                let effectStone = stoneControllers[index] as EffectStone
                where effectStone != null
                select effectStone);
            }

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
