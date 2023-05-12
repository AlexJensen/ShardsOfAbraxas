using Abraxas.Cards.Controllers;
using Abraxas.Cards.Data;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using UnityEngine;
using Zenject;

namespace Abraxas.Cards.Factories
{
    class CardFactory : IFactory<CardData, ICardController>
    {
        readonly DiContainer _container;
        readonly Card.Settings _cardSettings;

        public CardFactory(DiContainer container, Card.Settings cardSettings)
        {
            _container = container;
            _cardSettings = cardSettings;
        }

        public ICardController Create(CardData data)
        {
            var cardGameObject = _container.InstantiatePrefab(_cardSettings.cardPrefab);
            var cardView = cardGameObject.GetComponent<CardView>();
            var cardDragListener = cardGameObject.GetComponent<CardDragListener>();
            var cardMouseOverListener = cardGameObject.GetComponent<CardMouseOverListener>();
            var cardController = _container.Instantiate<CardController>();
            var cardModel = _container.Instantiate<CardModel>();
            var cardDragHandler = _container.Instantiate<CardDragHandler>();
            var cardMouseOverHandler = _container.Instantiate<CardMouseOverHandler>();

            cardModel.Initialize(data);
            cardController.Initialize(cardModel, cardModel, cardView);
            cardView.Initialize(cardModel, cardController);
            cardDragHandler.Initialize(cardController);
            cardMouseOverHandler.Initialize(cardController);
            cardDragListener.Initialize(cardDragHandler);
            cardMouseOverListener.Initialize(cardMouseOverHandler);

            return cardController;
        }
    }
}
