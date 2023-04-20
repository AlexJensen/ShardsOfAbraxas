using Abraxas.Cards;
using Abraxas.Zones.Overlays;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Hands
{
    public class Hand : Zone
    {
        #region Settings
        Settings _settings;
        [Serializable]
        public class Settings
        {
            public float MoveCardToHandTime;
        }
        #endregion

        #region Dependencies
        IOverlayManager _overlayManager;

        [Inject]
        void Construct(Settings settings, IOverlayManager dragManager)
        {
            _settings = settings;
            _overlayManager = dragManager;
        }
        #endregion

        #region Fields
        [SerializeField]
        Player player;
        List<Card> _cardList;
        CardPlaceholder _cardPlaceholder;
        bool cardReturning;
        #endregion

        #region Properties
        public CardPlaceholder CardPlaceholder => _cardPlaceholder = _cardPlaceholder != null ? _cardPlaceholder : Cards.GetComponentInChildren<CardPlaceholder>();
        public List<Card> CardList => _cardList = (_cardList ??= Cards.GetComponentsInChildren<Card>().ToList());

        public Player Player { get => player;}

        public override Zones ZoneType => Zones.HAND;

        public override float MoveCardTime { get => _settings.MoveCardToHandTime; }
        #endregion

        #region Methods
        private void Awake()
        {
            CardPlaceholder.Reset();
        }

        void Update()
        {
            if (_overlayManager.Card != null && _overlayManager.Card.Controller == Player && _overlayManager.Card.Zone == Zones.HAND)
            {
                UpdateCardPlaceholderPosition();
            }
            else if (!cardReturning && CardPlaceholder.isActiveAndEnabled)
            {
                CardPlaceholder.Hide();
            }
        }

        public void RemoveCard(Card card)
        {
            if (CardList.Contains(card))
            {
                CardPlaceholder.transform.SetSiblingIndex(CardList.IndexOf(card));
                CardPlaceholder.gameObject.SetActive(true);
                CardPlaceholder.SnapToMaxHeight();
                CardList.Remove(card);
            }
        }

        public override IEnumerator MoveCardToZone(Card card, int index = 0)
        {
            _overlayManager.AddCard(card);
            CardPlaceholder.gameObject.SetActive(true);
            CardPlaceholder.transform.SetSiblingIndex(index);
            cardReturning = true;
            yield return CardPlaceholder.ScaleToMaxSize();
            yield return card.RectTransformMover.MoveToFitRectangle(CardPlaceholder.CardPlaceholderRect, MoveCardTime);
            cardReturning = false;
            AddCardAtPlaceholder(card);
        }

        public void AddCardAtPlaceholder(Card card)
        {
            card.Zone = Zones.HAND;
            _overlayManager.RemoveCard(card);
            CardList.Insert(CardPlaceholder.transform.GetSiblingIndex(), card);
            card.transform.SetParent(CardPlaceholder.transform.parent);
            card.transform.SetSiblingIndex(CardPlaceholder.transform.GetSiblingIndex());
            CardPlaceholder.Reset();
        }

        private void UpdateCardPlaceholderPosition()
        {
            foreach (Card card in CardList)
            {
                if (Player == Player.Player1 ?
                    Input.mousePosition.y > card.transform.position.y :
                    Input.mousePosition.y < card.transform.position.y)
                {
                    CardPlaceholder.UpdateIndex(CardList.IndexOf(card));
                    break;
                }
                if (CardList.IndexOf(card) == CardList.Count - 1)
                {
                    CardPlaceholder.UpdateIndex(CardList.IndexOf(card) + 1);
                    break;
                }
            }
            CardPlaceholder.UpdatePosition();
        }
        #endregion
    }
}