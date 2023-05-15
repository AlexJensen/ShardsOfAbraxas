using Abraxas.Cards;
using Abraxas.Cards.Controllers;
using Abraxas.Cards.Views;
using Abraxas.Zones.Hands.Controllers;
using Abraxas.Zones.Overlays;
using Abraxas.Zones.Views;
using System.Collections;
using UnityEngine;
using Zenject;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Hands.Views
{
    class HandView : ZoneView, IHandView
    { 

        #region Fields
        [SerializeField]
        RectTransform HandParent;

        CardPlaceholder _cardPlaceholder;
        bool cardReturningToPlaceholder;
        #endregion

        #region Properties
        public CardPlaceholder CardPlaceholder => _cardPlaceholder = _cardPlaceholder != null ? _cardPlaceholder : GetComponentInChildren<CardPlaceholder>();


        public override float MoveCardTime { get => AnimationSettings.MoveCardToHandTime; }

        public int CardPlaceholderSiblingIndex => CardPlaceholder.transform.GetSiblingIndex();
        #endregion

        #region Methods
        private void Awake()
        {
            CardPlaceholder.Reset();
        }

        void Update()
        {
            if (OverlayManager.Card != null && OverlayManager.Card.Controller.Owner == Player && OverlayManager.Card.Controller.Zone is IHandController)
            {
                UpdateCardPlaceholderPosition();
            }
            else if (!cardReturningToPlaceholder && CardPlaceholder.isActiveAndEnabled)
            {
                CardPlaceholder.Hide();
            }
        }

        public void RemoveCard(ICardController card)
        {
            CardPlaceholder.transform.SetSiblingIndex(card.View.Transform.GetSiblingIndex());
            CardPlaceholder.gameObject.SetActive(true);
            CardPlaceholder.SnapToMaxHeight();
        }

        public override IEnumerator MoveCardToZone(ICardView card, int index = 0)
        {
            OverlayManager.SetCard(card);
            CardPlaceholder.gameObject.SetActive(true);
            CardPlaceholder.transform.SetSiblingIndex(index);
            cardReturningToPlaceholder = true;
            yield return CardPlaceholder.ScaleToMaxSize();
            yield return card.RectTransformMover.MoveToFitRectangle(CardPlaceholder.CardPlaceholderRect, MoveCardTime);
            cardReturningToPlaceholder = false;
            AddCardAtPlaceholder(card);
        }

        public void AddCardAtPlaceholder(ICardView card)
        {
            OverlayManager.ClearCard(card);
            card.Transform.SetParent(CardPlaceholder.transform.parent);
            card.Transform.SetSiblingIndex(CardPlaceholder.transform.GetSiblingIndex());
            CardPlaceholder.Reset();
        }

        private void UpdateCardPlaceholderPosition()
        {
            var cardList = Model.CardList;
            foreach (ICardController card in cardList)
            {
                if (Player == Player.Player1 ?
                    Input.mousePosition.y > card.View.Transform.position.y :
                    Input.mousePosition.y < card.View.Transform.position.y)
                {
                    CardPlaceholder.UpdateIndex(cardList.IndexOf(card));
                    break;
                }
                if (cardList.IndexOf(card) == cardList.Count - 1)
                {
                    CardPlaceholder.UpdateIndex(cardList.IndexOf(card) + 1);
                    break;
                }
            }
            CardPlaceholder.UpdatePosition();
        }
        #endregion
    }
}