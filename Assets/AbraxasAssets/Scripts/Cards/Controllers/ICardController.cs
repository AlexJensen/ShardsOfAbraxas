using Abraxas.Cells.Controllers;
using Abraxas.StatBlocks.Controllers;
using Abraxas.StatusEffects;
using Abraxas.Stones;
using Abraxas.Stones.Controllers;
using Abraxas.UI;
using Abraxas.Unity.Interfaces;
using Abraxas.Zones.Controllers;
using Abraxas.Zones.Fields.Controllers;
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
        void ChangeScale(PointF pointF, float scaleCardToOverlayTime);
        IEnumerator CheckDeath();
        IEnumerator Combat(IFieldController field);
        bool DeterminePlayability();
        IEnumerator Fight(ICardController opponent);
        string GetCostText();
        IEnumerator MoveToCell(ICellController cell, float moveCardTime);
        IEnumerator PassHomeRow();
        IEnumerator Attack(ICardController opponent);
        IEnumerator PlayAnimationClip(UnityEngine.AnimationClip clip, UnityEngine.Color color, bool flip);
        void SetCardPositionToMousePosition();
        void SetToInitialScale();
        void UpdatePlayabilityAndCostText();
        void RequestApplyStatusEffect(IStatusEffect effect);
        bool RequestHasStatusEffect<T>() where T : IStatusEffect;
        void RequestRemoveStatusEffect<T>() where T : IStatusEffect;
        void OnDestroy();
    }
}
