using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.Cells.Controllers;
using Abraxas.Core;
using Abraxas.Events;
using Abraxas.GameStates;
using Abraxas.Health.Managers;
using Abraxas.Manas;
using Abraxas.Players.Managers;
using Abraxas.StatBlocks.Controllers;
using Abraxas.StatusEffects;
using Abraxas.StatusEffects.Types;
using Abraxas.Stones;
using Abraxas.Stones.Controllers;
using Abraxas.UI;
using Abraxas.Unity.Interfaces;
using Abraxas.Zones.Controllers;
using Abraxas.Zones.Fields.Controllers;
using Abraxas.Zones.Hands.Controllers;
using Abraxas.Zones.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
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
        readonly ICardControllerInternal _aggregator;
        readonly ICardModel _model;
        readonly ICardView _view;

        public IGameStateManager GameStateManager => Aggregator.GameStateManager;

        public IPlayerManager PlayerManager => Aggregator.PlayerManager;

        public IZoneManager ZoneManager => Aggregator.ZoneManager;

        public IPlayerHealthManager HealthManager => Aggregator.HealthManager;


        public BaseCardController(ICardControllerInternal controller, ICardModel model, ICardView view)
        {
            _aggregator = controller;
            _model = model;
            _view = view;
        }
        #endregion

        List<ManaType> _lastManas = new();

        #region ICardController Properties

        public List<ManaType> LastManas { get => _lastManas; set => _lastManas = value; }
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
        public virtual ICardControllerInternal Aggregator => _aggregator;

        public bool EnablePreMovementRangedAttack
        {
            get
            {
                return _aggregator.EnablePreMovementRangedAttack;
            }
            set
            {
                _aggregator.EnablePreMovementRangedAttack = value;
            }
        }

        public bool EnablePostMovementRangedAttack
        {
            get
            {
                return _aggregator.EnablePostMovementRangedAttack;
            }
            set
            {
                _aggregator.EnablePostMovementRangedAttack = value;
            }
        }

        public bool CanBeAttackedRanged
        {

            get => _aggregator.CanBeAttackedRanged;
            set => _aggregator.CanBeAttackedRanged = value;
        }


        public bool HasAttacked
        {
            get => _aggregator.HasAttacked;
            set => _aggregator.HasAttacked = value;
        }

        public bool CanFight { get => Aggregator.CanFight; set => Aggregator.CanFight = value; }


       
        #endregion

        #region Status Effects
        public void ApplyStatusEffect(IStatusEffect effect)
        {
            _aggregator.ApplyStatusEffect(effect);
        }

        public bool HasStatusEffect<T>() where T : IStatusEffect => _aggregator.HasStatusEffect<T>();
        public void RemoveStatusEffect<T>() where T : IStatusEffect
        {
            _aggregator.RemoveStatusEffect<T>();
        }

        public void RequestApplyStatusEffect(IStatusEffect effect) => ApplyStatusEffect(effect);
        public bool RequestHasStatusEffect<T>() where T : IStatusEffect => HasStatusEffect<T>();
        public void RequestRemoveStatusEffect<T>() where T : IStatusEffect => RemoveStatusEffect<T>();
        #endregion

        #region Core Methods
        public bool DeterminePlayability()
        {
            if (Aggregator.Zone is not IHandController ||
                !(Aggregator.GameStateManager.State is BeforeCombatState or AfterCombatState) ||
                Aggregator.PlayerManager.ActivePlayer != Aggregator.Owner)
            {
                return false;
            }

            foreach (var _ in from pair in Aggregator.TotalCosts
                              let manaPair = LastManas.FirstOrDefault(m => m.Type == pair.Key)
                              where manaPair == null || pair.Value > manaPair.Amount
                              select new { })
            {
                return false;
            }
            return true;
        }

        

        public IEnumerator Attack(ICardController opponent, bool ranged)
        {
            if (Aggregator.HasAttacked) yield break;
            if (opponent.Owner == Aggregator.Owner) yield break;

            yield return opponent.PlayAnimationClip(_view.AttackAnimation, Aggregator.StatBlock.Color, Aggregator.PlayerManager.LocalPlayer == Player.Player2);
            yield return Aggregator.DealDamage(opponent, Aggregator.StatBlock.Stats.ATK); 
            Aggregator.HasAttacked = true;
        }

        public IEnumerator CheckDeath()
        {
            if (StatBlock.Stats.DEF > 0 || Zone is not IFieldController) yield break;
            yield return Aggregator.ZoneManager.MoveCardFromFieldToGraveyard(Aggregator, Owner);
        }

        public IEnumerator PreCombat()
        {
            Aggregator.HasAttacked = false;
            Aggregator.CanBeAttackedRanged = true;
            Aggregator.CanFight = true;
            Aggregator.EnablePreMovementRangedAttack = false;
            Aggregator.EnablePostMovementRangedAttack = Aggregator.StatBlock.Stats.RNG > 0;
            yield break;
        }

        public IEnumerator Combat(IFieldController field)
        {            
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
            yield return opponent.TakeDamage(this, amount);
        }

        public IEnumerator TakeDamage(ICardController source, int amount)
        {
            var stats = Aggregator.StatBlock.Stats;
            stats.DEF -= amount;

            Aggregator.StatBlock.Stats = stats;
            yield return Aggregator.CheckDeath();
        }

        public IEnumerator Fight(ICardController opponent)
        {
            if (CanFight)
            {
                yield return Utilities.WaitForCoroutines(
                    opponent.Attack(Aggregator, false),
                    Aggregator.Attack(opponent, false));
            }
        }

        public virtual ICardController CheckRangedAttack(IFieldController field, Point movement)
        {
            if (field != Zone) return null;
            Point destination = new(
                            Math.Clamp(Aggregator.Cell.FieldPosition.X + (Aggregator.StatBlock.Stats.RNG * Math.Sign(movement.X)), 0, field.FieldGrid[0].Count - 1),
                            Aggregator.Cell.FieldPosition.Y);

            for (int i = Aggregator.Cell.FieldPosition.X + Math.Sign(movement.X); i != destination.X + Math.Sign(movement.X); i += Math.Sign(movement.X))
            {
                if (i > field.FieldGrid[0].Count - 1 || i < 0) break;
                if (field.FieldGrid[Cell.FieldPosition.Y][i].CardsOnCell <= 0) continue;
                destination.X = i - Math.Sign(movement.X);
                var collided = field.FieldGrid[Cell.FieldPosition.Y][i].GetCardAtIndex(0) as ICardControllerInternal;
                if (collided.CanBeAttackedRanged)
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
                var target = Aggregator.CheckRangedAttack(field, new Point(Aggregator.Owner == Player.Player1 ? Aggregator.StatBlock.Stats.RNG : -Aggregator.StatBlock.Stats.RNG, 0));
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
            var movement = new Point(Aggregator.Owner == Player.Player1 ? Aggregator.StatBlock.Stats.SPD : -Aggregator.StatBlock.Stats.SPD, 0);
            Point destination = new(
                Math.Clamp(Aggregator.Cell.FieldPosition.X + movement.X, 0, field.FieldGrid[0].Count - 1),
                Aggregator.Cell.FieldPosition.Y);

            ICardController collided = null;
            var fieldGrid = field.FieldGrid;

            // Movement and collision detection
            for (int i = Aggregator.Cell.FieldPosition.X + Math.Sign(movement.X); i != destination.X + Math.Sign(movement.X); i += Math.Sign(movement.X))
            {
                if (fieldGrid[Aggregator.Cell.FieldPosition.Y][i].CardsOnCell <= 0) continue;
                destination.X = i - Math.Sign(movement.X);
                collided = fieldGrid[Aggregator.Cell.FieldPosition.Y][i].GetCardAtIndex(0);
                break;
            }

            // Move the card if necessary
            if (destination != Aggregator.Cell.FieldPosition)
            {
                yield return field.MoveCardToCell(Aggregator, fieldGrid[destination.Y][destination.X]);
            }

            // Handle collision
            if (collided != null)
            {
                yield return Aggregator.Fight(collided);
            }
            else if (fieldGrid[destination.Y][destination.X].Player != Owner && fieldGrid[destination.Y][destination.X].Player != Player.Neutral)
            {
                yield return Aggregator.PassHomeRow();
            }


            yield break;
        }

        public IEnumerator MoveToCell(ICellController cell, float moveCardTime)
        {
            yield return _view.MoveToCell(cell, moveCardTime);
        }

        public IEnumerator PassHomeRow()
        {
            yield return Aggregator.HealthManager.ModifyPlayerHealth(Aggregator.Owner ==
               Player.Player1 ? Player.Player2 : Player.Player1, -Aggregator.StatBlock.Stats.ATK);
            yield return Aggregator.ZoneManager.MoveCardFromFieldToDeck(Aggregator, Aggregator.Owner, 0, true);
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
            _view.UpdateCostTextWithManaTypes(LastManas, Aggregator.TotalCosts, Aggregator.DeterminePlayability(), Aggregator.Zone is IHandController);
        }

        public void OnDestroy()
        {
            // Default: no cleanup needed
        }
        #endregion

        #region Events Handling
        public IEnumerator OnEventRaised(Event_ManaModified eventData)
        {
            LastManas = eventData.Mana.ManaTypes;
            Aggregator.UpdatePlayabilityAndCostText();
            yield break;
        }

        public IEnumerator OnEventRaised(Event_GameStateEntered eventData)
        {
            if (LastManas != null)
            {
                Aggregator.UpdatePlayabilityAndCostText();
            }
            yield break;
        }

        public IEnumerator OnEventRaised(Event_ActivePlayerChanged eventData)
        {
            if (LastManas != null)
            {
                Aggregator.UpdatePlayabilityAndCostText();
            }
            yield break;
        }

        public IEnumerator OnEventRaised(Event_CardChangedZones eventData)
        {
            Aggregator.StatBlock.ShowSymbols = false;
            // Apply summoning sickness when a card enters the field
            if (Aggregator.Zone is IFieldController)
            {
                Aggregator.StatBlock.ShowSymbols = true;
                RequestApplyStatusEffect(new StatusEffect_SummoningSickness());
            }
            if (LastManas != null)
            {
                Aggregator.UpdatePlayabilityAndCostText();
            }
            yield break;
            
        }

        public bool ShouldReceiveEvent(Event_ManaModified eventData) => eventData.Mana.Player == Aggregator.Owner && eventData.Mana.ManaTypes != null;
        public bool ShouldReceiveEvent(Event_CardChangedZones eventData) => eventData.Card.Equals(Aggregator);
        public bool ShouldReceiveEvent(Event_GameStateEntered eventData) => Aggregator.Zone is IHandController;
        public bool ShouldReceiveEvent(Event_ActivePlayerChanged eventData) => Aggregator.Zone is IHandController;
        #endregion
    }
}
