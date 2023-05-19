using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.Cells.Controllers;
using Abraxas.StatBlocks.Models;
using Abraxas.Stones;
using Abraxas.Zones.Controllers;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Abraxas.Cards.Controllers
{
    public interface ICardController
    {
        ICardModelReader Model { get; }
        ICardView View { get; }

        public Dictionary<StoneType, int> TotalCosts { get; }
        IZoneController Zone { get; set; }
        bool Hidden { get; set; }
        Players.Players Owner { get; }
        Players.Players OriginalOwner { get; }
        Point FieldPosition { get; set; }
        ICellController Cell { get; set; }
        IStatBlockModel StatBlock { get; }

        IEnumerator CheckDeath();
        IEnumerator Combat();
        IEnumerator Fight(ICardController opponent);
        IEnumerator PassHomeRow();
        void ScaleToRectangle(PointF dimensions, float time);
        void AddToOverlay();
    }
}
