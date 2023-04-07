using Abraxas.Behaviours.Cards;
using Abraxas.Behaviours.Game;
using Abraxas.Behaviours.Zones.Decks;
using Abraxas.Behaviours.Zones.Drags;
using Abraxas.Behaviours.Zones.Fields;
using Abraxas.Behaviours.Zones.Graveyards;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Abraxas.Behaviours.Zones.Hands
{
    public class Hand : Zone
    {
        #region Fields
        [SerializeField]
        Transform cards_t;
        List<Card> cards;
        CardPlaceholder cardPlaceholder;
        Deck deck;
        Graveyard graveyard;
        [SerializeField]
        GameManager.Player player;
        internal bool cardReturning;
        #endregion

        #region Properties
        public CardPlaceholder CardPlaceholder => cardPlaceholder = cardPlaceholder != null ? cardPlaceholder : cards_t.GetComponentInChildren<CardPlaceholder>();
        public Deck Deck => deck = deck != null ? deck : deck = GetComponentInChildren<Deck>();
        public List<Card> Cards => cards = (cards ??= cards_t.GetComponentsInChildren<Card>().ToList());
        public Graveyard Graveyard => graveyard = graveyard != null ? graveyard : graveyard = GetComponentInChildren<Graveyard>();

        public GameManager.Player Player { get => player; set => player = value; }

        public override ZoneManager.Zones ZoneType => ZoneManager.Zones.HAND;
        #endregion

        #region Unity Methods
        private void Awake()
        {
            CardPlaceholder.Reset();
        }

        void Update()
        {
            if (DragManager.Instance.card != null && DragManager.Instance.card.Controller == Player)
            {
                Cards.Remove(DragManager.Instance.card);
                cardPlaceholder.gameObject.SetActive(true);
                UpdateCardPlaceholderPosition();
            }
            else if (!cardReturning && cardPlaceholder.isActiveAndEnabled)
            {
                CardPlaceholder.Hide();
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Moves a card into the current position of the card placeholder and replaces it.
        /// </summary>
        /// <param name="card">Card to move.</param>
        public void AddCardAtPlaceholder(Card card)
        {
            Cards.Insert(CardPlaceholder.transform.GetSiblingIndex(), card);
            card.transform.SetParent(CardPlaceholder.transform.parent);
            card.transform.SetSiblingIndex(CardPlaceholder.transform.GetSiblingIndex());
            CardPlaceholder.Reset();
        }

        private void UpdateCardPlaceholderPosition()
        {
            foreach (Card card in Cards)
            {
                if (Player == GameManager.Player.Player1 ?
                    Input.mousePosition.y > card.transform.position.y :
                    Input.mousePosition.y < card.transform.position.y)
                {
                    CardPlaceholder.UpdateIndex(Cards.IndexOf(card));
                    break;
                }
                if (Cards.IndexOf(card) == Cards.Count - 1)
                {
                    CardPlaceholder.UpdateIndex(Cards.IndexOf(card) + 1);
                    break;
                }
            }
            cardPlaceholder.CheckPosition();
        }

        public void RemoveCard(Card card)
        {
            if (Cards.Contains(card))
            {
                CardPlaceholder.transform.SetSiblingIndex(Cards.IndexOf(card));
                CardPlaceholder.gameObject.SetActive(true);
                CardPlaceholder.SnapToMaxHeight();
                Cards.Remove(card);
            }
        }

        public IEnumerator DrawCardsFromLibrary(int amount = 1, int index = 0)
        {
            for (int i = 0; i < amount; i++)
            {
                Card card = Deck.DrawCard(index);
                cardReturning = true;
                CardPlaceholder.transform.SetSiblingIndex(0);
                CardPlaceholder.gameObject.SetActive(true);
                yield return StartCoroutine(CardPlaceholder.ScaleToMaxSize());
                CardPlaceholder.SnapToMaxHeight();
                yield return StartCoroutine(MoveCardToZone(card));
            }
        }

        public override IEnumerator MoveCardToZone(Card card)
        {
            card.Zone = ZoneManager.Zones.HAND;
            cardReturning = true;
            FieldManager.Instance.RemoveFromField(card);
            yield return card.MoveToFitRectangle(CardPlaceholder.CardPlaceholderRect);
            cardReturning = false;
            AddCardAtPlaceholder(card);
        }
        #endregion
    }
}