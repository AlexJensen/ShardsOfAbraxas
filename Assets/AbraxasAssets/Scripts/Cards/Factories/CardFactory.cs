using Abraxas.Cards.Controllers;
using Abraxas.Cards.Data;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.StackBlocks.Views;
using Abraxas.StatBlocks.Controllers;
using Abraxas.StatBlocks.Models;
using Abraxas.Stones.Controllers;
using Abraxas.Stones.Models;
using Abraxas.Stones.Views;
using System.Collections.Generic;
using Unity.Netcode;
using Zenject;
using Player = Abraxas.Players.Players;

namespace Abraxas.Cards.Factories
{
    class CardFactory : IFactory<CardData, Player, ICardController>
    {
        #region Dependencies
        readonly DiContainer _container;
        readonly Card.Settings _cardSettings;
        public CardFactory(DiContainer container, Card.Settings cardSettings)
        {
            _container = container;
            _cardSettings = cardSettings;
        }
        #endregion

        #region Methods
        public ICardController Create(CardData data, Player player)
        {
            var gameObject = _container.InstantiatePrefab(_cardSettings.cardPrefab);

            var view = gameObject.GetComponent<CardView>();
            var dragListener = gameObject.GetComponent<CardDragListener>();
            var mouseOverListener = gameObject.GetComponent<CardMouseOverView>();
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

            List<IStoneModel> stoneModels = new();
            foreach (var stoneData in data.Stones)
            {
                var stoneModel = _container.Instantiate<StoneModel>();
                var stoneController = _container.Instantiate<StoneController>();
                var stoneView = _container.InstantiateComponent<StoneView>(gameObject);
                stoneModel.Initialize(stoneData);
                stoneController.Initialize(stoneModel);
                stoneView.Initialize(stoneModel, stoneController);
                stoneModels.Add(stoneModel);
            }

            model.Initialize(data, statBlockModel, stoneModels);
            controller.Initialize(model, view);
            view.Initialize(model, controller);

            dragHandler.Initialize(dragListener, controller);
            dragListener.Initialize(dragHandler);
            mouseOverHandler.Initialize(controller);
            mouseOverListener.Initialize(mouseOverHandler);

            gameObject.GetComponent<NetworkObject>().Spawn();
            return controller;
        }
        #endregion
    }
}
