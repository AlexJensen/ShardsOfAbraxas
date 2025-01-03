﻿using Abraxas.Cards.Controllers;
using Abraxas.Stones;
using Abraxas.Zones.Views;
using System.Collections.Generic;
using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Models
{

    public interface IZoneModel
    {
        #region Properties
        List<ICardController> CardList { get; }
        Player Player { get; set; }
        ZoneType Type { get; }
        #endregion

        #region Methods
        void AddCard(ICardController card, int index = 0);
        void RemoveCard(int index);
        void RemoveCard(ICardController card);
        IEnumerable<ICardController> GetCardsForPlayer(Player player);
        Dictionary<StoneType, int> GetTotalCostOfZone();
        void Shuffle();
        void Initialize<TView>(TView view) where TView : IZoneView;
        #endregion
    }
}
