using Abraxas.Cards.Controllers;
using Abraxas.Zones.Views;
using System.Collections;
using Unity.Netcode;
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


        protected override float MoveCardTime { get => NetworkManager.Singleton.IsServer ? 0 : AnimationSettings.MoveCardToHandTime; }

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
            yield return card.RectTransformMover.MoveToFitRectangle(CardPlaceholder.CardPlaceholderRect, NetworkManager.Singleton.IsServer ? 0 : MoveCardTime);
            cardReturningToPlaceholder = false;
            card.TransformManipulator.Transform.SetParent(CardPlaceholder.transform.parent);
            card.TransformManipulator.Transform.SetSiblingIndex(CardPlaceholder.transform.GetSiblingIndex());
            CardPlaceholder.Reset();
            OverlayManager.ClearCard(card);
        }

        public void UpdateCardPlaceholderPosition()
        {
            var cardList = Model.CardList;
            foreach (ICardController card in cardList)
            {
                if (Input.mousePosition.y > card.TransformManipulator.Transform.position.y)
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

        public override void AddCardToHolder(ICardController card, int index = 0)
        {

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