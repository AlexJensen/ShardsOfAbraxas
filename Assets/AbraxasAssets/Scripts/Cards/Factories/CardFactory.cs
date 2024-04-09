using Abraxas.Cards.Controllers;
using Abraxas.Cards.Data;
using Abraxas.Cards.Managers;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.StackBlocks.Views;
using Abraxas.StatBlocks.Controllers;
using Abraxas.StatBlocks.Models;
using Abraxas.Stones.Controllers;
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
        public CardFactory(DiContainer container, Card.Settings cardSettings, ICardManager cardManager, StoneController.Factory stoneFactory)
        {
            _container = container;
            _cardSettings = cardSettings;
            _cardManager = cardManager;
            _stoneFactory = stoneFactory;
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
            var statBlockModel = _container.Instantiate<StatBlockModel>();
            var statBlockController = _container.Instantiate<StatBlockController>();
            var statBlockView = gameObject.GetComponent<StatBlockView>();

            statBlockModel.Initialize(data.StatBlock);
            statBlockController.Initialize(statBlockModel);
            statBlockView.Initialize(statBlockModel, statBlockController);

            List<IStoneController> stoneControllers = new();
            if (data.Stones != null)
            {
                foreach (var stoneData in data.Stones)
                {
                    var stoneController = _stoneFactory.Create(stoneData.RuntimeStoneData);
                    stoneControllers.Add(stoneController);
                }
            }

            model.Initialize(data, statBlockController, stoneControllers);
            controller.Initialize(model, view);
            view.Initialize(model);

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
