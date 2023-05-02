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
    public class Deck : Zone
    {
        #region Settings
        Settings _deckSettings;
        [Serializable]
        public class Settings
        {
            public float MoveCardToDeckTime;
        }
        #endregion

        #region Dependencies
        [Inject]
        public void Construct(Settings deckSettings)
        {
            _deckSettings = deckSettings;
        }
        #endregion

        #region Fields
        [SerializeField]
        Player player;
        
        #endregion

        #region Properties
        public Player Player { get => player; }
        public override float MoveCardTime { get => _deckSettings.MoveCardToDeckTime; }
        public override Zones ZoneType => Zones.DECK;
        #endregion

        #region Methods
        public Dictionary<StoneType, int> GetDeckCost()
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

        internal void Shuffle()
        {
            foreach (Transform card in Cards)
            {
                card.transform.SetSiblingIndex(Random.Range(0, Cards.childCount));
                card.GetComponent<Card>().Hidden = true;
            }
        }
        #endregion
    }
}