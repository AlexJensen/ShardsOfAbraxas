using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Deck : MonoBehaviour
{
    [SerializeField]
    Transform t_cards;

    private void Update()
    {
        foreach (Transform card in t_cards)
        {
            card.localScale = Vector3.zero;
        }
    }

    internal Card DrawCard(int index = 0)
    {
        Card card = t_cards.GetChild(index).GetComponent<Card>();
        card.transform.localScale = Vector3.one;
        card.transform.position = transform.position;
        card.transform.SetParent(Drag.Instance.transform);
        card.zone = Card.Zone.HAND;
        return card;
    }

    public void Shuffle()
    {
        foreach (Transform card in t_cards)
        {
            card.SetSiblingIndex(Random.Range(0, t_cards.childCount));
        }
    }

    public Dictionary<StoneData.StoneType, int> GetTotalDeckCosts()
    {
        Dictionary<StoneData.StoneType, int> totalCost = new();
        Card[] cards = GetComponentsInChildren<Card>();
        foreach (Card card in cards)
        {
            foreach (KeyValuePair<StoneData.StoneType, int> manaAmount in card.TotalCosts)
            {
                if (!totalCost.ContainsKey(manaAmount.Key))
                {
                    totalCost.Add(manaAmount.Key, manaAmount.Value);
                }
                else
                {
                    totalCost[manaAmount.Key] += manaAmount.Value;
                }
            }
        }
        return totalCost;
    }
}
