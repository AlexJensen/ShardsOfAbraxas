﻿using Abraxas.Cards.Controllers;
using Abraxas.Stones;
using Abraxas.Zones.Views;
using System.Collections.Generic;
using System.Linq;
using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Models
{
    abstract class ZoneModel :IZoneModel
    {
        #region Dependencies
        public virtual void Initialize<TView>(TView view) where TView : IZoneView
        {

        }
        #endregion

        #region Fields
        readonly List<ICardController> _cardList = new();
        #endregion

        #region Properties
        public List<ICardController> CardList => _cardList;
        public Player Player { get; set; }
        #endregion

        #region Methods
        public void AddCard(ICardController card, int index = 0)
        {
            _cardList.Insert(index, card);
        }

        public ICardController RemoveCard(ICardController card)
        {
            _cardList.Remove(card);
            return card;
        }

        public ICardController RemoveCard(int index)
        {
            ICardController card = _cardList[index];
            _cardList.Remove(card);
            return card;
        }

        public IEnumerable<ICardController> GetCardsForPlayer(Player player)
        {
            return _cardList.Where(s => s.Owner == player);
        }

        public Dictionary<StoneType, int> GetTotalCostOfZone()
        {
            Dictionary<StoneType, int> totalCost = new();
            foreach (var manaAmount in from ICardController card in _cardList
                                       from manaAmount in card.Model.TotalCosts
                                       select manaAmount)
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

            return totalCost;
        }

        public virtual void Shuffle()
        {
            int count = _cardList.Count;
            for (int i = count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);

                ICardController temp = _cardList[i];
                _cardList[i] = _cardList[j];
                _cardList[j] = temp;
            }
        }


        #endregion
    }
}
