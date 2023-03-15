using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    #region Fields
    [SerializeField]
    Transform cards_t;
    List<Card> cards;
    CardPlaceholder cardPlaceholder;
    Deck deck;
    Graveyard graveyard;

    public Game.Player player;
    internal bool cardReturning;
    #endregion

    #region Properties
    public CardPlaceholder CardPlaceholder => cardPlaceholder = cardPlaceholder != null ? cardPlaceholder : cards_t.GetComponentInChildren<CardPlaceholder>();
    public Deck Deck => deck = deck != null ? deck : deck = GetComponentInChildren<Deck>();
    public List<Card> Cards => cards = (cards ??= cards_t.GetComponentsInChildren<Card>().ToList());
    public Graveyard Graveyard => graveyard = graveyard != null ? graveyard : graveyard = GetComponentInChildren<Graveyard>();
    #endregion

    #region Unity Methods
    private void Awake()
    {
        CardPlaceholder.Reset();
    }

    void Update()
    {
        if (Drag.Instance.card != null && Drag.Instance.card.controller == player)
        {
            Cards.Remove(Drag.Instance.card);
            cardPlaceholder.gameObject.SetActive(true);

            foreach (Card card in Cards)
            {
                if (player == Game.Player.Player1 ?
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

    public void RemoveCard(Card card)
    {
        if (Cards.Contains(card))
        {
            CardPlaceholder.transform.SetSiblingIndex(Cards.IndexOf(card));
            CardPlaceholder.SnapToMaxHeight();
            Cards.Remove(card);
        }
    }

    internal IEnumerator DrawCardFromLibrary(int amount = 1, int index = 0)
    {
        Card card = Deck.DrawCard(index);
        cardReturning = true;
        CardPlaceholder.transform.SetSiblingIndex(0);
        CardPlaceholder.gameObject.SetActive(true);
        yield return StartCoroutine(CardPlaceholder.ScaleToMaxSize());
        CardPlaceholder.SnapToMaxHeight();
        yield return null;
        yield return StartCoroutine(card.MoveToHand(this));
        if (amount > 1) yield return StartCoroutine(DrawCardFromLibrary(amount - 1, index));
    }
    #endregion
}
