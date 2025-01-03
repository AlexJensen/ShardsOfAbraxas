﻿using Abraxas.Cards.Controllers;
using Abraxas.Core;
using System.Collections;
using System.Drawing;
using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Managers
{
    public interface IZoneManager : IManager
    {
        #region Methods
        IEnumerator MoveCardFromFieldToDeck(ICardController card, Player player, int index = 0, bool reverse = false);
        IEnumerator MoveCardFromFieldToGraveyard(ICardController card, Player player);
        IEnumerator MoveCardFromHandToCell(ICardController card, Point fieldPosition);
        IEnumerator MoveCardFromDeckToGraveyard(ICardController card, Player player);
        IEnumerator MoveCardFromDeckToHand(ICardController card, Player player);
        #endregion
    }
}
