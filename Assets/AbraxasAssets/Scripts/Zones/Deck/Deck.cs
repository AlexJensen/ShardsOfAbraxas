using Abraxas.Cards;
using Abraxas.Stones;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

using Random = UnityEngine.Random;
using Player = Abraxas.Players.Players;
using Zenject;
using System;

namespace Abraxas.Zones.Decks
{
    //[ExecuteInEditMode]
    public class Deck : Zone
    {
        #region Settings
        Settings _settings;
        [Serializable]
        public class Settings
        {
            public float MoveCardToDeckTime;
        }
        #endregion

        #region Dependencies
        [Inject]
        public void Construct(Settings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Fields
        [SerializeField]
        Player player;
        public override Zones ZoneType => Zones.DECK;
        #endregion

        #region Properties
        public Player Player { get => player; }
        public override float MoveCardTime { get => _settings.MoveCardToDeckTime; }
        #endregion

        #region Methods
        public void AddCard(Card card, int index = 0)
        {
            card.transform.localScale = Vector3.zero;
            card.transform.position = transform.position;
            card.Zone = Zones.DECK;
            card.transform.parent = Cards.transform;
            card.transform.SetSiblingIndex(index);
        }
        public Card RemoveCard(int index = 0)
        {
            Card card = Cards.GetChild(index).GetComponent<Card>();
            card.transform.localScale = Vector3.one;
            card.transform.position = transform.position;
            return card;
        }
        public Dictionary<StoneType, int> GetTotalDeckCosts()
        {
            Dictionary<StoneType, int> totalCost = new();
            Card[] cards = GetComponentsInChildren<Card>();
            foreach (Card card in cards)
            {
                foreach (var manaAmount in card.TotalCosts)
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
        #endregion

        #region Server Methods
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
        #endregion
    }
}