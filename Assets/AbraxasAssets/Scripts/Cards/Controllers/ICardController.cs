using Abraxas.Cells.Controllers;
using Abraxas.Manas;
using Abraxas.StatBlocks.Controllers;
using Abraxas.StatusEffects;
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
        string Title { get; set; }
        public Dictionary<StoneType, int> TotalCosts { get; }
        IZoneController Zone { get; set; }
        IZoneController PreviousZone { get; set; }
        bool Hidden { get; set; }
        Players.Players Owner { get; set; }
        Players.Players OriginalOwner { get; set; }
        ICellController Cell { get; set; }
        IStatBlockController StatBlock { get; }
        ITransformManipulator TransformManipulator { get; }
        IImageManipulator ImageManipulator { get; }
        RectTransformMover RectTransformMover { get; }
        List<ManaType> LastManas { get; set; }

        void ApplyStatusEffect(IStatusEffect effect);
        void ChangeScale(PointF pointF, float scaleCardToOverlayTime);
        IEnumerator CheckDeath();
        IEnumerator Combat();
        bool DeterminePlayability();
        IEnumerator Fight(ICardController opponent);
        string GetCostText();
        bool HasStatusEffect<T>() where T : IStatusEffect;
        IEnumerator MoveToCell(ICellController cell, float moveCardTime);
        IEnumerator PassHomeRow();
        IEnumerator RangedAttack(ICardController opponent);
        void RemoveStatusEffect<T>() where T : IStatusEffect;
        void SetCardPositionToMousePosition();
        void SetToInitialScale();
        void UpdatePlayabilityAndCostText();
    }
}
