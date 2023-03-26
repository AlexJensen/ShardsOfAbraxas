using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class Deck : NetworkBehaviour
{
    [SerializeField]
    Transform cards;

    public Transform Cards { get => cards;}

    private void Update()
    {
        foreach (Transform card in Cards)
        {
            card.localScale = Vector3.zero;
        }
    }

    internal Card DrawCard(int index = 0)
    {
        Card card = Cards.GetChild(index).GetComponent<Card>();
        card.transform.localScale = Vector3.one;
        card.transform.position = transform.position;
        card.transform.SetParent(Drag.Instance.transform);
        card.zone = Card.Zone.HAND;
        return card;
    }

    [ServerRpc]
    public void ShuffleServerRpc()
    {
        foreach (Transform card in Cards)
        {
            card.SetSiblingIndex(Random.Range(0, Cards.childCount));
        }
    }

    [ClientRpc]
    private void ShuffleClientRpc()
    {
        // On the clients, the shuffle method just re-parents the cards
        for (int i = 0; i < Cards.childCount; i++)
        {
            Cards.GetChild(i).SetSiblingIndex(i);
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
