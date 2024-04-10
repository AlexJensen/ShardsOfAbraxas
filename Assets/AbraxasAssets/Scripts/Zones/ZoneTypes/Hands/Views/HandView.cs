using Abraxas.Cards.Controllers;
using Abraxas.Zones.Views;
using System.Collections;
using UnityEngine;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Hands.Views
{
    class HandView : ZoneView, IHandView
    {
        #region Fields
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

        public override void RemoveCardFromHolder(ICardController card)
        {
            CardPlaceholder.transform.SetSiblingIndex(card.TransformManipulator.Transform.GetSiblingIndex());
            CardPlaceholder.gameObject.SetActive(true);
            CardPlaceholder.SnapToMaxHeight();
        }

        public override IEnumerator MoveCardToZone(ICardController card, int index = 0)
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

        public void AddCardAtPlaceholder(ICardController card)
        {
            OverlayManager.ClearCard(card);
            card.TransformManipulator.Transform.SetParent(CardPlaceholder.transform.parent);
            card.TransformManipulator.Transform.SetSiblingIndex(CardPlaceholder.transform.GetSiblingIndex());
            CardPlaceholder.Reset();
        }

        public void UpdateCardPlaceholderPosition()
        {
            var cardList = Model.CardList;
            foreach (ICardController card in cardList)
            {
                if (Player == Player.Player1 ?
                    Input.mousePosition.y > card.TransformManipulator.Transform.position.y :
                    Input.mousePosition.y < card.TransformManipulator.Transform.position.y)
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

        public void HidePlaceholder()
        {
            if (!cardReturningToPlaceholder && CardPlaceholder.isActiveAndEnabled)
            {
                CardPlaceholder.Hide();
            }
        }
        #endregion
    }
}