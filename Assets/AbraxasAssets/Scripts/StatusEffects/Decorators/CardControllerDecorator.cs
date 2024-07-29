using Abraxas.Cells.Controllers;
using Abraxas.StatBlocks.Controllers;
using Abraxas.StatusEffects;
using Abraxas.Stones.Controllers;
using Abraxas.Stones;
using Abraxas.Unity.Interfaces;
using Abraxas.Zones.Controllers;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Player = Abraxas.Players.Players;

namespace Abraxas.Cards.Controllers
{
    abstract class CardControllerDecorator : ICardController
    {
        protected ICardController _innerController;

        public CardControllerDecorator(ICardController innerController)
        {
            _innerController = innerController;
        }

        public virtual IStatBlockController StatBlock => _innerController.StatBlock;
        public virtual List<IStoneController> Stones => _innerController.Stones;
        public virtual string Title { get => _innerController.Title;  }
        public virtual Player Owner { get => _innerController.Owner; }
        public virtual Player OriginalOwner { get => _innerController.OriginalOwner; }
        public virtual Dictionary<StoneType, int> TotalCosts => _innerController.TotalCosts;
        public virtual ICellController Cell { get => _innerController.Cell; set => _innerController.Cell = value; }
        public virtual IZoneController Zone { get => _innerController.Zone; set => _innerController.Zone = value; }
        public virtual IZoneController PreviousZone { get => _innerController.PreviousZone; set => _innerController.PreviousZone = value; }
        public virtual bool Hidden { get => _innerController.Hidden; set => _innerController.Hidden = value; }
        public virtual ITransformManipulator TransformManipulator => _innerController.TransformManipulator;
        public virtual IImageManipulator ImageManipulator => _innerController.ImageManipulator;
        public virtual RectTransformMover RectTransformMover => _innerController.RectTransformMover;

        public virtual IEnumerator PassHomeRow() => _innerController.PassHomeRow();
        public virtual IEnumerator Fight(ICardController opponent) => _innerController.Fight(opponent);
        public virtual void ApplyStatusEffect(IStatusEffect effect) => _innerController.ApplyStatusEffect(effect);
        public virtual bool HasStatusEffect<T>() where T : IStatusEffect => _innerController.HasStatusEffect<T>();
        public virtual void RemoveStatusEffect<T>() where T : IStatusEffect => _innerController.RemoveStatusEffect<T>();
        public virtual IEnumerator RangedAttack(ICardController opponent) => _innerController.RangedAttack(opponent);
        public virtual IEnumerator CheckDeath() => _innerController.CheckDeath();
        public virtual IEnumerator Combat() => _innerController.Combat();
        public virtual void ChangeScale(PointF pointF, float scaleCardToOverlayTime) => _innerController.ChangeScale(pointF, scaleCardToOverlayTime);
        public virtual void SetToInitialScale() => _innerController.SetToInitialScale();
        public virtual void SetCardPositionToMousePosition() => _innerController.SetCardPositionToMousePosition();
        public virtual string GetCostText() => _innerController.GetCostText();
        public virtual IEnumerator MoveToCell(ICellController cell, float moveCardTime) => _innerController.MoveToCell(cell, moveCardTime);
        public virtual bool DeterminePlayability() => _innerController.DeterminePlayability();


    }
}