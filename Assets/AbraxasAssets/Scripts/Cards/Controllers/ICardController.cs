﻿using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.Stones;
using Abraxas.Zones.Fields;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Abraxas.Cards.Controllers
{
    public interface ICardController
    {
        ICardModelReader Model { get; }
        ICardView View { get; }
        ICardDragHandler DragHandler { get; }
        ICardMouseOverHandler MouseOverHandler { get; }
        public Dictionary<StoneType, int> TotalCosts { get; }
        Zones.Zones Zone { get; set; }
        bool Hidden { get; set; }
        Players.Players Owner { get; }
        Players.Players OriginalOwner { get; }
        Point FieldPosition { get; set; }
        ICellController Cell { get; set; }
        StatBlock StatBlock { get; }

        IEnumerator CheckDeath();
        IEnumerator Combat();
        IEnumerator Fight(ICardController opponent);
        IEnumerator PassHomeRow();
    }
}