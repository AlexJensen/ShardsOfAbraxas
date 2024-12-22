using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.Cells.Controllers;
using Abraxas.Events;
using Abraxas.Events.Managers;
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
using Abraxas.Zones.Fields.Managers;
using Abraxas.Zones.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Zenject;
using Player = Abraxas.Players.Players;

namespace Abraxas.Cards.Controllers
{
    /// <summary>
    /// BaseCardController is the concrete component in the decorator pattern.
    /// It provides default, baseline implementations for all ICardController and ICardControllerInternal methods and operates as the outlet back to the card controller aggregator.
    /// No delegation occurs here; decorators wrap around this to add or modify behavior. DefaultCardController is the first decorator in the chain and sits above this base class.
    /// </summary>
    class BaseCardController : ICardControllerInternal
    {
        #region Dependencies
        readonly ICardControllerInternal _controller;
        readonly ICardModel _model;
        readonly ICardView _view;

        protected IPlayerManager _playerManager;
        protected IGameStateManager _gameStateManager;
        protected IZoneManager _zoneManager;
        protected IEventManager _eventManager;
        protected IPlayerHealthManager _healthManager;
        protected IFieldManager _fieldManager;

        public BaseCardController(ICardControllerInternal controller, ICardModel model, ICardView view)
        {
            _controller = controller;
            _model = model;
            _view = view;
        }


        [Inject]
        public void Construct(
            IPlayerManager playerManager,
            IGameStateManager gameStateManager,
            IZoneManager zoneManager,
            IEventManager eventManager,
            IPlayerHealthManager healthManager,
            IFieldManager fieldManager)
        {
            _playerManager = playerManager;
            _gameStateManager = gameStateManager;
            _zoneManager = zoneManager;
            _eventManager = eventManager;
            _healthManager = healthManager;
            _fieldManager = fieldManager;
        }
        #endregion

        #region ICardController Properties
        public List<IStoneController> Stones => _model.Stones;
        public string Title { get => _model.Title; set => _model.Title = value; }
        public Dictionary<StoneType, int> TotalCosts => _model.TotalCosts;
        public IZoneController Zone { get => _model.Zone; set => _model.Zone = value; }
        public IZoneController PreviousZone { get; set; } // Default: not used, can remain null
        public bool Hidden { get => _model.Hidden; set => _model.Hidden = value; }
        public Player Owner { get => _model.Owner; set => _model.Owner = value; }
        public Player OriginalOwner { get => _model.OriginalOwner; set => _model.OriginalOwner = value; }
        public ICellController Cell { get => _model.Cell; set => _model.Cell = value; }
        public virtual IStatBlockController StatBlock => _model.StatBlock;

        public ITransformManipulator TransformManipulator => (ITransformManipulator)_view;
        public IImageManipulator ImageManipulator => (IImageManipulator)_view;
        public RectTransformMover RectTransformMover => _view.RectTransformMover;
        public virtual ICardControllerInternal Aggregator => _controller;

        public bool EnablePreMovementRangedAttack
        {
            get
            {
                return _controller.EnablePreMovementRangedAttack;
            }
            set
            {
                _controller.EnablePreMovementRangedAttack = value;
            }
        }

        public bool EnablePostMovementRangedAttack
        {
            get
            {
                return _controller.EnablePostMovementRangedAttack;
            }
            set
            {
                _controller.EnablePostMovementRangedAttack = value;
            }
        }

        public bool HasAttacked
        {
            get
            {
                return _controller.HasAttacked;
            }
            set
            {
                _controller.HasAttacked = value;
            }
        }
        #endregion

        #region Status Effects
        public void ApplyStatusEffect(IStatusEffect effect)
        {
            _controller.ApplyStatusEffect(effect);
        }

        public bool HasStatusEffect<T>() where T : IStatusEffect => _controller.HasStatusEffect<T>();
        public void RemoveStatusEffect<T>() where T : IStatusEffect
        {
            _controller.RemoveStatusEffect<T>();
        }

        public void RequestApplyStatusEffect(IStatusEffect effect) => ApplyStatusEffect(effect);
        public bool RequestHasStatusEffect<T>() where T : IStatusEffect => HasStatusEffect<T>();
        public void RequestRemoveStatusEffect<T>() where T : IStatusEffect => RemoveStatusEffect<T>();
        #endregion

        #region Core Methods
        public bool DeterminePlayability()
        {
            return false;
        }

        public bool CanBeAttackedRanged()
        {
            return true;
        }

        public IEnumerator Attack(ICardController opponent, bool ranged)
        {
            if (HasAttacked) yield break;
            if (opponent.Owner == Owner) yield break;

            yield return opponent.PlayAnimationClip(_view.AttackAnimation, StatBlock.Color, _playerManager.LocalPlayer == Player.Player2);
            yield return DealDamage(opponent, StatBlock.Stats.ATK); 
            HasAttacked = true;
        }

        public IEnumerator CheckDeath()
        {
            if (StatBlock.Stats.DEF > 0 || Zone is not IFieldController) yield break;
            yield return _zoneManager.MoveCardFromFieldToGraveyard(Aggregator, Owner);
        }

        public IEnumerator Combat(IFieldController field)
        {
            HasAttacked = false;
            EnablePreMovementRangedAttack = false;
            EnablePostMovementRangedAttack = StatBlock.Stats.RNG > 0;
            yield return Aggregator.PreMovementAction(field);
            yield return Aggregator.MoveAndHandleCollisions(field);
            yield return Aggregator.PostMovementAction(field);
        }

        public virtual IEnumerator PreMovementAction(IFieldController field)
        {
            yield return Aggregator.RangedAttack(field, Aggregator.EnablePreMovementRangedAttack);
        }

        public virtual IEnumerator PostMovementAction(IFieldController field)
        {
            yield return Aggregator.RangedAttack(field, Aggregator.EnablePostMovementRangedAttack);
        }

        public IEnumerator DealDamage(ICardController opponent, int amount)
        {
            yield return opponent.TakeDamage(amount);
        }

        public IEnumerator TakeDamage(int amount)
        {
            var stats = Aggregator.StatBlock.Stats;
            stats.DEF -= amount;

            Aggregator.StatBlock.Stats = stats;
            yield return Aggregator.CheckDeath();
        }

        public IEnumerator Fight(ICardController opponent)
        {
            yield break;
        }

        public virtual ICardController CheckRangedAttack(IFieldController field, Point movement)
        {
            if (field != Zone) return null;
            Point destination = new(
                            Math.Clamp(Cell.FieldPosition.X + (StatBlock.Stats.RNG * Math.Sign(movement.X)), 0, field.FieldGrid[0].Count - 1),
                            Cell.FieldPosition.Y);

            for (int i = Cell.FieldPosition.X + Math.Sign(movement.X); i != destination.X + Math.Sign(movement.X); i += Math.Sign(movement.X))
            {
                if (i > field.FieldGrid[0].Count - 1 || i < 0) break;
                if (field.FieldGrid[Cell.FieldPosition.Y][i].CardsOnCell <= 0) continue;
                destination.X = i - Math.Sign(movement.X);
                var collided = field.FieldGrid[Cell.FieldPosition.Y][i].GetCardAtIndex(0);
                if (collided.CanBeAttackedRanged())
                {
                    return collided;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        public virtual IEnumerator RangedAttack(IFieldController field, bool doAttack)
        {
            if (doAttack)
            {
                var target = Aggregator.CheckRangedAttack(field, new Point(Owner == Player.Player1 ? StatBlock.Stats.RNG : -StatBlock.Stats.RNG, 0));
                if (target != null)
                {
                    yield return Aggregator.Attack(target, true);
                }
            }
        }

        public string GetCostText()
        {
            return _view.GetCostText();
        }

        public IEnumerator MoveAndHandleCollisions(IFieldController field)
        {
            yield break;
        }

        public IEnumerator MoveToCell(ICellController cell, float moveCardTime)
        {
            yield return _view.MoveToCell(cell, moveCardTime);
        }

        public IEnumerator PassHomeRow()
        {
            yield return _healthManager.ModifyPlayerHealth(Owner ==
               Player.Player1 ? Player.Player2 : Player.Player1, -StatBlock.Stats.ATK);
            yield return _zoneManager.MoveCardFromFieldToDeck(Aggregator, Owner, 0, true);
        }

        public IEnumerator PlayAnimationClip(AnimationClip clip, UnityEngine.Color color, bool flip)
        {

            yield return _view.PlayAnimation(clip, color, flip);
        }

        public void ChangeScale(PointF pointF, float scaleCardToOverlayTime) => _view.ChangeScale(pointF, scaleCardToOverlayTime);

        public void SetCardPositionToMousePosition()
        {
            _view.SetCardPositionToMousePosition();
        }

        public void SetToInitialScale()
        {
            _view.SetToInitialScale();
        }

        public void UpdatePlayabilityAndCostText()
        {
           // Default: no-op
        }

        public void OnDestroy()
        {
            // Default: no cleanup needed
        }
        #endregion

        #region Events Handling (Base Does Nothing)
        public IEnumerator OnEventRaised(Event_ManaModified eventData)
        {
            yield break;
        }

        public IEnumerator OnEventRaised(Event_GameStateEntered eventData)
        {
            yield break;
        }

        public IEnumerator OnEventRaised(Event_ActivePlayerChanged eventData)
        {
            yield break;
        }

        public IEnumerator OnEventRaised(Event_CardChangedZones eventData)
        {
            yield break;
        }

        public bool ShouldReceiveEvent(Event_ManaModified eventData) => false;
        public bool ShouldReceiveEvent(Event_CardChangedZones eventData) => false;
        public bool ShouldReceiveEvent(Event_GameStateEntered eventData) => false;
        public bool ShouldReceiveEvent(Event_ActivePlayerChanged eventData) => false;
        #endregion
    }
}
