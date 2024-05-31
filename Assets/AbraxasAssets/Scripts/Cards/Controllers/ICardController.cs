using Abraxas.Cells.Controllers;
using Abraxas.StatBlocks.Controllers;
using Abraxas.Stones;
using Abraxas.Stones.Controllers;
using Abraxas.Unity.Interfaces;
using Abraxas.Zones.Controllers;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Abraxas.Cards.Controllers
{
    /// <summary>
    /// ICardController is an interface for Card Controllers.
    /// </summary>
    public interface ICardController
    {
        List<IStoneController> Stones { get; }
        string Title { get; }
        public Dictionary<StoneType, int> TotalCosts { get; }
        IZoneController Zone { get; set; }
        IZoneController PreviousZone { get; set; }
        bool Hidden { get; set; }
        Players.Players Owner { get; }
        Players.Players OriginalOwner { get; }
        ICellController Cell { get; set; }
        IStatBlockController StatBlock { get; }
        ITransformManipulator TransformManipulator { get; }
        IImageManipulator ImageManipulator { get; }
        RectTransformMover RectTransformMover { get; }

        void ChangeScale(PointF pointF, float scaleCardToOverlayTime);
        IEnumerator CheckDeath();
        IEnumerator Combat();
        IEnumerator Fight(ICardController opponent);
        string GetCostText();
        IEnumerator MoveToCell(ICellController cell, float moveCardTime);
        IEnumerator PassHomeRow();
        void SetCardPositionToMousePosition();
    }
}
