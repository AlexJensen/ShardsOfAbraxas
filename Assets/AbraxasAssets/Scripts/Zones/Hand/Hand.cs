using Abraxas.Behaviours.Cards;
using Abraxas.Behaviours.Game;
using Abraxas.Behaviours.Players;
using Abraxas.Behaviours.Zones.Decks;
using Abraxas.Behaviours.Zones.Drags;
using Abraxas.Behaviours.Zones.Fields;
using Abraxas.Behaviours.Zones.Graveyards;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Abraxas.Behaviours.Zones.Hands
{
    public class Hand : Zone
    {
        #region Dependency Injections
        [Inject] readonly FieldManager _fieldManager;
        #endregion

        #region Fields

        [SerializeField]
        Player player;
        List<Card> _cardList;
        CardPlaceholder _cardPlaceholder;
        Deck _deck;
        Graveyard _graveyard;

        bool cardReturning;
        #endregion

        #region Properties
        public CardPlaceholder CardPlaceholder => _cardPlaceholder = _cardPlaceholder != null ? _cardPlaceholder : Cards.GetComponentInChildren<CardPlaceholder>();
        public Deck Deck => _deck = _deck != null ? _deck : _deck = GetComponentInChildren<Deck>();
        public List<Card> CardList => _cardList = (_cardList ??= Cards.GetComponentsInChildren<Card>().ToList());
        public Graveyard Graveyard => _graveyard = _graveyard != null ? _graveyard : _graveyard = GetComponentInChildren<Graveyard>();

        public Player Player { get => player; set => player = value; }

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
                CardList.Remove(DragManager.Instance.card);
                CardPlaceholder.gameObject.SetActive(true);
                UpdateCardPlaceholderPosition();
            }
            else if (!cardReturning && CardPlaceholder.isActiveAndEnabled)
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
            CardPlaceholder.CheckPosition();
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
            _fieldManager.RemoveFromField(card);
            yield return card.MoveToFitRectangle(CardPlaceholder.CardPlaceholderRect);
            cardReturning = false;
            AddCardAtPlaceholder(card);
        }
        #endregion
    }
}