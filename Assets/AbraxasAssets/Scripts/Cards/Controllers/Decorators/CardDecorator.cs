using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.Cells.Controllers;
using Abraxas.Events;
using Abraxas.GameStates;
using Abraxas.Health.Managers;
using Abraxas.Players.Managers;
using Abraxas.StatBlocks.Controllers;
using Abraxas.StatusEffects;
using Abraxas.Stones;
using Abraxas.Stones.Controllers;
using Abraxas.UI;
using Abraxas.Unity.Interfaces;
using Abraxas.Zones.Controllers;
using Abraxas.Zones.Fields.Controllers;
using Abraxas.Zones.Managers;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Player = Abraxas.Players.Players;

namespace Abraxas.Cards.Controllers
{
    /// <summary>
    /// The default DefaultBehaviorDecorator class functions as the baseline for expected card behaviour and lays over the BaseCardController layer. It provides all specific game logic for card actions and effects.
    /// </summary>
    class CardDecorator : ICardControllerInternal
                                    
    {
        #region Dependencies
        protected ICardControllerInternal _innerController;
        protected ICardModel _model;
        protected ICardView _view;

        public CardDecorator(ICardControllerInternal innerController, ICardModel model, ICardView view)
        {
            _innerController = innerController;
            _model = model;
            _view = view;
        }

        public virtual void OnDestroy()
        {
            InnerController.OnDestroy();
        }
        #endregion

        #region Flags
        public bool EnablePreMovementRangedAttack { get => InnerController.EnablePreMovementRangedAttack; set => InnerController.EnablePreMovementRangedAttack = value; }
        public bool EnablePostMovementRangedAttack { get => InnerController.EnablePostMovementRangedAttack; set => InnerController.EnablePostMovementRangedAttack = value; }
        public bool CanFight { get => InnerController.CanFight; set => InnerController.CanFight = value; }
        public bool CanPassHomeRow { get => InnerController.CanPassHomeRow; set => InnerController.CanPassHomeRow = value; }
        public bool CanBeAttackedRanged { get => InnerController.CanBeAttackedRanged; set => InnerController.CanBeAttackedRanged = value; }
        public bool HasAttacked { get => InnerController.HasAttacked; set => InnerController.HasAttacked = value; }
        public bool CanAlliedRangedAttacksShootThrough { get => InnerController.CanAlliedRangedAttacksShootThrough; set => InnerController.CanAlliedRangedAttacksShootThrough = value; }
        #endregion

        #region Properties
        public virtual ICardControllerInternal Aggregator => InnerController.Aggregator;
        public IGameStateManager GameStateManager => InnerController.GameStateManager;
        public IPlayerManager PlayerManager => InnerController.PlayerManager;
        public IZoneManager ZoneManager => InnerController.ZoneManager;
        public IPlayerHealthManager HealthManager => InnerController.HealthManager;
        public virtual IStatBlockController StatBlock => InnerController.StatBlock;
        public virtual List<IStoneController> Stones => InnerController.Stones;
        public virtual string Title { get => InnerController.Title; set => InnerController.Title = value; }
        public virtual Player Owner { get => InnerController.Owner; set => InnerController.Owner = value; }
        public virtual Player OriginalOwner { get => InnerController.OriginalOwner; set => InnerController.OriginalOwner = value; }
        public virtual Dictionary<StoneType, int> TotalCosts => InnerController.TotalCosts;
        public virtual ICellController Cell { get => InnerController.Cell; set => InnerController.Cell = value; }
        public virtual IZoneController Zone { get => InnerController.Zone; set => InnerController.Zone = value; }
        public virtual IZoneController PreviousZone { get => InnerController.PreviousZone; set => InnerController.PreviousZone = value; }
        public virtual bool Hidden { get => InnerController.Hidden; set => InnerController.Hidden = value; }
        public virtual ITransformManipulator TransformManipulator => InnerController.TransformManipulator;
        public virtual IImageManipulator ImageManipulator => InnerController.ImageManipulator;
        public virtual RectTransformMover RectTransformMover => InnerController.RectTransformMover;       
        public ICardControllerInternal InnerController { get => _innerController; }

        #endregion

        #region Status Effects
        public virtual void RequestApplyStatusEffect(IStatusEffect effect) => ApplyStatusEffect(effect);
        public virtual bool RequestHasStatusEffect<T>() where T : IStatusEffect => HasStatusEffect<T>();
        public virtual void RequestRemoveStatusEffect<T>() where T : IStatusEffect => RemoveStatusEffect<T>();
        public void ApplyStatusEffect(IStatusEffect effect) => InnerController.ApplyStatusEffect(effect);
        public bool HasStatusEffect<T>() where T : IStatusEffect => InnerController.HasStatusEffect<T>();
        public void RemoveStatusEffect<T>() where T : IStatusEffect => InnerController.RemoveStatusEffect<T>();
        #endregion

        #region Methods
        public virtual IEnumerator PassHomeRow()
        {
            yield return InnerController.PassHomeRow();
        }

        public virtual IEnumerator Fight(ICardController opponent)
        {
            yield return InnerController.Fight(opponent);
        }

        public virtual IEnumerator Attack(ICardController opponent, bool ranged)
        {
            yield return InnerController.Attack(opponent, ranged);
        }

        public virtual IEnumerator DealDamage(ICardController opponent, int amount)
        {
            yield return InnerController.DealDamage(opponent, amount);
        }

        public virtual IEnumerator TakeDamage(ICardController source, int amount)
        {
            yield return InnerController.TakeDamage(source, amount);
        }

        public virtual IEnumerator CheckDeath()
        {

            yield return InnerController.CheckDeath();
        }

        public virtual IEnumerator PreCombat()
        {
            return InnerController.PreCombat();
        }

        public virtual IEnumerator Combat(IFieldController field)
        {
            yield return InnerController.Combat(field);
        }


        public virtual IEnumerator PreMovementAction(IFieldController field)
        {
            yield return InnerController.PreMovementAction(field);
        }

       
        public virtual IEnumerator MoveAndHandleCollisions(IFieldController field)
        {
           yield return InnerController.MoveAndHandleCollisions(field);
        }


        public virtual IEnumerator PostMovementAction(IFieldController field)
        {
            yield return InnerController.PostMovementAction(field);
        }

        public virtual IEnumerator RangedAttack(IFieldController field, bool doAttack)
        {
            yield return InnerController.RangedAttack(field, doAttack);
        }

        public virtual ICardController CheckRangedAttack(IFieldController field, Point movement)
        {
            return InnerController.CheckRangedAttack(field, movement);
        }

        public virtual bool DeterminePlayability() => InnerController.DeterminePlayability();

        public virtual void ChangeScale(PointF pointF, float scaleCardToOverlayTime) => InnerController.ChangeScale(pointF, scaleCardToOverlayTime);
        public virtual void SetToInitialScale() => InnerController.SetToInitialScale();
        public virtual void SetCardPositionToMousePosition() => InnerController.SetCardPositionToMousePosition();
        public virtual string GetCostText() => InnerController.GetCostText();

        public virtual IEnumerator PlayAnimationClip(UnityEngine.AnimationClip clip, UnityEngine.Color color, bool flip) => InnerController.PlayAnimationClip(clip, color, flip);

        
        public virtual IEnumerator MoveToCell(ICellController cell, float moveCardTime) => InnerController.MoveToCell(cell, moveCardTime);

        public virtual void UpdatePlayabilityAndCostText() => InnerController.UpdatePlayabilityAndCostText();
        #endregion

        #region Event Listeners
        public virtual IEnumerator OnEventRaised(Event_ManaModified eventData)
        {
            yield return InnerController.OnEventRaised(eventData);
        }

        public virtual bool ShouldReceiveEvent(Event_ManaModified eventData) => InnerController.ShouldReceiveEvent(eventData);

        public virtual IEnumerator OnEventRaised(Event_CardChangedZones eventData)
        {
            yield return InnerController.OnEventRaised(eventData);
        }

        public virtual bool ShouldReceiveEvent(Event_CardChangedZones eventData) => InnerController.ShouldReceiveEvent(eventData);

        public virtual IEnumerator OnEventRaised(Event_GameStateEntered eventData)
        {
            yield return InnerController.OnEventRaised(eventData);
        }

        public virtual bool ShouldReceiveEvent(Event_GameStateEntered eventData) => InnerController.ShouldReceiveEvent(eventData);

        public virtual IEnumerator OnEventRaised(Event_ActivePlayerChanged eventData)
        {
            yield return InnerController.OnEventRaised(eventData);
        }
        public virtual bool ShouldReceiveEvent(Event_ActivePlayerChanged eventData) => InnerController.ShouldReceiveEvent(eventData);

        public virtual Point CalculateDestination(IFieldController field, Point movement)
        {
            return InnerController.CalculateDestination(field, movement);
        }

        public virtual ICardController FindCollisionAlongPath(IFieldController field, ref Point destination, Point movement)
        {
            return InnerController.FindCollisionAlongPath(field, ref destination, movement);
        }

        public virtual Point GetMovementVector()
        {
            return InnerController.GetMovementVector();
        }

        public virtual IEnumerator HandlePostMovementState(IFieldController field, ICardController collided, Point destination)
        {
            return InnerController.HandlePostMovementState(field, collided, destination);
        }

        public virtual IEnumerator MoveToDestinationCell(IFieldController field, Point destination)
        {
            return InnerController.MoveToDestinationCell(field, destination);
        }

        public virtual bool IsCellAvailable(ICellController cell)
        {
            return InnerController.IsCellAvailable(cell);
        }
        #endregion
    }
}