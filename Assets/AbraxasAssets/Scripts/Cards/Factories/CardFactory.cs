using Abraxas.Cards.Controllers;
using Abraxas.Cards.Data;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.StackBlocks.Views;
using Abraxas.StatBlocks.Controllers;
using Abraxas.StatBlocks.Models;
using Zenject;
using Player = Abraxas.Players.Players;

namespace Abraxas.Cards.Factories
{
    class CardFactory : IFactory<CardData, Player, ICardController>
    {
        readonly DiContainer _container;
        readonly Card.Settings _cardSettings;
        public CardFactory(DiContainer container, Card.Settings cardSettings)
        {
            _container = container;
            _cardSettings = cardSettings;
        }

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
            var statBlockView = gameObject.GetComponent<StatBlockView>();

            statBlockView.Initialize(statBlockModel);
            model.Initialize(data, statBlockModel);
            controller.Initialize(model, model, view);
            view.Initialize(model, controller);
            dragHandler.Initialize(controller);
            mouseOverHandler.Initialize(controller);
            dragListener.Initialize(dragHandler);
            mouseOverListener.Initialize(mouseOverHandler);

            return controller;
        }
    }
}
