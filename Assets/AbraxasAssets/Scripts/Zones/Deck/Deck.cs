using Abraxas.Behaviours.Cards;
using Abraxas.Behaviours.Data;
using Abraxas.Behaviours.Zones.Drags;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Abraxas.Behaviours.Zones.Decks
{
    [ExecuteInEditMode]
    public class Deck : Zone
    {
        [SerializeField]
        Transform cards;

        public Transform Cards { get => cards; }

        public override ZoneManager.Zones ZoneType => ZoneManager.Zones.DECK;

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
            card.transform.SetParent(DragManager.Instance.transform);
            card.Zone = ZoneManager.Zones.HAND;
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

        internal override void AddCard(Card card, int index = -1)
        {
            base.AddCard(card, index);
            if (index == -1)
            {
                ShuffleServerRpc();
            }
        }
    }
}