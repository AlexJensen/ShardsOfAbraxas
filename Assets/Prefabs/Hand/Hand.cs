using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    public GameData.Players player;

    List<Card> cards;
    CardPlaceholder cardPlaceholder;

    RectTransform rectTransform;
    internal bool cardReturning;

    public CardPlaceholder CardPlaceholder { get => cardPlaceholder; }

    private void Start()
    {
        cards = GetComponentsInChildren<Card>().ToList();
        cardPlaceholder = GetComponentInChildren<CardPlaceholder>();
        CardPlaceholder.Reset();
    }

    public void AddCardAtPlaceholder(Card card)
    {
        cards.Insert(CardPlaceholder.transform.GetSiblingIndex(), card);
        card.transform.SetParent(CardPlaceholder.transform.parent);
        card.transform.SetSiblingIndex(CardPlaceholder.transform.GetSiblingIndex());
        CardPlaceholder.Reset();
    }

    // Update is called once per frame
    void Update()
    {
        if (Drag.Instance.card != null && Drag.Instance.card.controller == player)
        {
            cards.Remove(Drag.Instance.card);
            cardPlaceholder.Initialize();

            foreach (Card card in cards)
            {
                if (player == GameData.Players.PLAYER_1?
                    Input.mousePosition.y > card.transform.position.y:
                    Input.mousePosition.y < card.transform.position.y)
                {
                    CardPlaceholder.UpdateIndex(cards.IndexOf(card));
                    break;
                }
                if (cards.IndexOf(card) == cards.Count - 1)
                {
                    CardPlaceholder.UpdateIndex(cards.IndexOf(card) + 1);
                    break;
                }
            }
            cardPlaceholder.CheckPosition();
        }
        else if (!cardReturning)
        {
            cardPlaceholder.Hide();
        }      
    }
}
