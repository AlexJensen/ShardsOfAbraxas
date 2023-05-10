using Abraxas.Stones;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;
using Player = Abraxas.Players.Players;
using Zenject;
using System;
using Abraxas.Cards.Views;

namespace Abraxas.Zones.Decks
{
    public class Deck : ZoneView
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
            ICardView[] cards = GetComponentsInChildren<ICardView>();
            foreach (ICardView card in cards)
            {
                foreach (var manaAmount in card.Model.TotalCosts)
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
                card.GetComponent<ICardView>().Controller.Hidden = true;
            }
        }
        #endregion
    }
}