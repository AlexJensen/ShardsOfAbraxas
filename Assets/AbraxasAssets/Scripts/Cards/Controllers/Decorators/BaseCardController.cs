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
    /// No delegation occurs here; decorators wrap around this to add or modify behavior. 
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

        #region Flags
        public bool EnablePreMovementRangedAttack
        {
            get => Aggregator.EnablePreMovementRangedAttack;
            set => Aggregator.EnablePreMovementRangedAttack = value;
        }

        public bool EnablePostMovementRangedAttack
        {
            get => Aggregator.EnablePostMovementRangedAttack;
            set => Aggregator.EnablePostMovementRangedAttack = value;
        }

        public bool CanBeAttackedRanged
        {

            get => Aggregator.CanBeAttackedRanged;
            set => Aggregator.CanBeAttackedRanged = value;
        }


        public bool HasAttacked
        {
            get => Aggregator.HasAttacked;
            set => Aggregator.HasAttacked = value;
        }

        public bool CanFight
        {
            get => Aggregator.CanFight;
            set => Aggregator.CanFight = value;
        }

        public bool CanAlliedRangedAttacksShootThrough
        {
            get => Aggregator.CanAlliedRangedAttacksShootThrough;
            set => Aggregator.CanAlliedRangedAttacksShootThrough = value;
        }

        public bool CanPassHomeRow
        {
            get => Aggregator.CanPassHomeRow;
            set => Aggregator.CanPassHomeRow = value;
        }

        #endregion

        #region Properties
        public List<ManaType> LastManas { get ; set; }
        public IZoneController PreviousZone { get; set; }
        public List<IStoneController> Stones => _model.Stones;
        public string Title { get => _model.Title; set => _model.Title = value; }
        public Dictionary<StoneType, int> TotalCosts => _model.TotalCosts;
        public IZoneController Zone { get => _model.Zone; set => _model.Zone = value; }
        public bool Hidden { get => _model.Hidden; set => _model.Hidden = value; }
        public Player Owner { get => _model.Owner; set => _model.Owner = value; }
        public Player OriginalOwner { get => _model.OriginalOwner; set => _model.OriginalOwner = value; }
        public ICellController Cell { get => _model.Cell; set => _model.Cell = value; }
        public virtual IStatBlockController StatBlock => _model.StatBlock;
        public ITransformManipulator TransformManipulator => (ITransformManipulator)_view;
        public IImageManipulator ImageManipulator => (IImageManipulator)_view;
        public RectTransformMover RectTransformMover => _view.RectTransformMover;
        public virtual ICardControllerInternal Aggregator => _aggregator;
        #endregion

        #region Status Effects
        public void ApplyStatusEffect(IStatusEffect effect)
        {
            Aggregator.ApplyStatusEffect(effect);
        }
        public bool HasStatusEffect<T>() where T : IStatusEffect => Aggregator.HasStatusEffect<T>();
        public void RemoveStatusEffect<T>() where T : IStatusEffect
        {
            Aggregator.RemoveStatusEffect<T>();
        }

        public void RequestApplyStatusEffect(IStatusEffect effect) => ApplyStatusEffect(effect);
        public bool RequestHasStatusEffect<T>() where T : IStatusEffect => HasStatusEffect<T>();
        public void RequestRemoveStatusEffect<T>() where T : IStatusEffect => RemoveStatusEffect<T>();
        #endregion

        #region Methods

        /// <summary>
        /// A card can only be played if the player is the active player, the game state is either of the two main phases, the card is in the player's hand, and the player has enough mana to pay the card's cost. 
        /// Note this only checks if this specific card can be played, another check to see if there are any available open cells must also be made by the field manager to conclusively determine if the card can be played.
        /// </summary>>
        public bool DeterminePlayability()
        {
            if (Aggregator.Zone is not IHandController ||
                !(Aggregator.GameStateManager.State is BeforeCombatState or AfterCombatState) ||
                Aggregator.PlayerManager.ActivePlayer != Aggregator.Owner)
            {
                return false;
            }

            if (LastManas == null) return false;

            foreach (var _ in from pair in Aggregator.TotalCosts
                              let manaPair = LastManas.FirstOrDefault(m => m.Type == pair.Key)
                              where manaPair == null || pair.Value > manaPair.Amount
                              select new { })
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// A card attacks an opponent's card. Normally cards can only attack once per combat step, ranged or melee.
        /// </summary>
        /// <param name="opponent"></param>
        /// <param name="ranged"></param>
        /// <returns></returns>
        public IEnumerator Attack(ICardController opponent, bool ranged)
        {
            if (Aggregator.HasAttacked) yield break;
            if (opponent.Owner == Aggregator.Owner) yield break;

            yield return opponent.PlayAnimationClip(_view.AttackAnimation, Aggregator.StatBlock.Color, Aggregator.PlayerManager.LocalPlayer == Player.Player2);
            yield return Aggregator.DealDamage(opponent, Aggregator.StatBlock.Stats.ATK); 
            Aggregator.HasAttacked = true;
        }

        /// <summary>
        /// A card is destroyed if its DEF is reduced to 0 or less while on the field. 
        /// </summary>
        /// <returns></returns>
        public IEnumerator CheckDeath()
        {
            if (StatBlock.Stats.DEF > 0 || Zone is not IFieldController) yield break;
            yield return Aggregator.ZoneManager.MoveCardFromFieldToGraveyard(Aggregator, Owner);
        }

        /// <summary>
        /// Default combat flags are set before combat begins.
        /// </summary>
        /// <returns></returns>
        public IEnumerator PreCombat()
        {
            Aggregator.HasAttacked = false;
            Aggregator.CanBeAttackedRanged = true;
            Aggregator.CanAlliedRangedAttacksShootThrough = false;
            Aggregator.CanFight = true;
            Aggregator.CanPassHomeRow = true;
            Aggregator.EnablePreMovementRangedAttack = false;
            Aggregator.EnablePostMovementRangedAttack = Aggregator.StatBlock.Stats.RNG > 0;
            yield break;
        }

        /// <summary>
        /// Combat is split into three phases: pre-movement, movement, and post-movement. Each phase can have different actions and effects applied to the card.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public IEnumerator Combat(IFieldController field)
        {            
            yield return Aggregator.PreMovementAction(field);
            yield return Aggregator.MoveAndHandleCollisions(field);
            yield return Aggregator.PostMovementAction(field);
        }

        /// <summary>
        /// Pre-movement actions are applied to the card before it moves on the field.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public IEnumerator PreMovementAction(IFieldController field)
        {
            yield return Aggregator.RangedAttack(field, Aggregator.EnablePreMovementRangedAttack);
        }

        /// <summary>
        /// After movement actions are applied to the card after it moves on the field.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public IEnumerator PostMovementAction(IFieldController field)
        {
            yield return Aggregator.RangedAttack(field, Aggregator.EnablePostMovementRangedAttack);
        }

        /// <summary>
        /// A card deals damage to an opponent's card.
        /// </summary>
        /// <param name="opponent"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public IEnumerator DealDamage(ICardController opponent, int amount)
        {
            yield return opponent.TakeDamage(this, amount);
        }

        /// <summary>
        /// A card takes damage from a source.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public IEnumerator TakeDamage(ICardController source, int amount)
        {
            var stats = Aggregator.StatBlock.Stats;
            stats.DEF -= amount;

            Aggregator.StatBlock.Stats = stats;
            yield return Aggregator.CheckDeath();
        }

        /// <summary>
        /// A card fights an opponent's card, each dealing damage to the other equal to its ATK value.
        /// </summary>
        /// <param name="opponent">Card to fight</param>
        /// <returns></returns>
        public IEnumerator Fight(ICardController opponent)
        {
            if (CanFight)
            {
                yield return Utilities.WaitForCoroutines(
                    opponent.Attack(Aggregator, false),
                    Aggregator.Attack(opponent, false));
            }
        }

        /// <summary>
        /// Ranged attacks allow a card to attack an opponent's card from a distance. The card will attack the first enemy card it encounters within its range.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="movement"></param>
        /// <returns></returns>
        public ICardController CheckRangedAttack(IFieldController field, Point movement)
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
                if (collided.Owner == Aggregator.Owner && collided.CanAlliedRangedAttacksShootThrough)
                {
                    continue;
                }
                else if (collided.CanBeAttackedRanged)
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

        /// <summary>
        /// Performs a ranged attack if able.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="doAttack"></param>
        /// <returns></returns>
        public IEnumerator RangedAttack(IFieldController field, bool doAttack)
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


        /// <summary>
        /// A card moves forward on the field equal to its SPD value during combat 
        /// and will fight enemy cards it comes in contact with during the movement.
        /// </summary>
        public IEnumerator MoveAndHandleCollisions(IFieldController field)
        {
            // 1. Calculate the desired destination.
            var movement = Aggregator.GetMovementVector();
            var destination = Aggregator.CalculateDestination(field, movement);

            // 2. Check whether a collision occurs along the path and 
            //    potentially adjust our final destination. 
            ICardController collided = Aggregator.FindCollisionAlongPath(field, ref destination, movement);

            // 3. Move the card to the destination cell (if needed).
            yield return Aggregator.MoveToDestinationCell(field, destination);

            // 4. After movement, handle either fighting a collided unit 
            //    or, if no collision, possibly passing the home row.
            yield return Aggregator.HandlePostMovementState(field, collided, destination);
        }

        /// <summary>
        /// Computes the movement vector (speed and direction) based on SPD and ownership.
        /// </summary>
        public Point GetMovementVector()
        {
            int spd = (Owner == Player.Player1) ? StatBlock.Stats.SPD : -StatBlock.Stats.SPD;
            return new Point(spd, 0);
        }

        /// <summary>
        /// Given the movement vector, calculates the final intended X-position 
        /// within the grid boundaries. 
        /// </summary>
        public Point CalculateDestination(IFieldController field, Point movement)
        {
            int gridWidth = field.FieldGrid[0].Count;
            int currentX = Cell.FieldPosition.X;
            int desiredX = currentX + movement.X;

            // Clamp within the field boundaries
            int clampedX = Mathf.Clamp(desiredX, 0, gridWidth - 1);

            return new Point(clampedX, Cell.FieldPosition.Y);
        }

        /// <summary>
        /// Looks for a collision along the path from our current position 
        /// to the final desired destination. 
        /// If a collision is found, updates 'destination' to the cell just before the collision
        /// and returns the encountered card.
        /// </summary>
        public ICardController FindCollisionAlongPath(IFieldController field, ref Point destination, Point movement)
        {
            if (movement.X == 0) return null;

            var fieldGrid = field.FieldGrid;
            int step = Math.Sign(movement.X);

            // Start one step ahead of our current X in the direction of movement
            for (int i = Cell.FieldPosition.X + step; i != destination.X + step; i += step)
            {
                // Check if there's a card in the path
                if (fieldGrid[Cell.FieldPosition.Y][i].CardsOnCell > 0)
                {
                    // Collided with a card at [Y][i]. 
                    // Adjust the destination to just before the collision.
                    destination.X = i - step;
                    return fieldGrid[Cell.FieldPosition.Y][i].GetCardAtIndex(0);
                }
            }

            return null; // No collision found
        }

        /// <summary>
        /// Moves this card to the 'destination' cell if the card is not 
        /// already in that position.
        /// </summary>
        public IEnumerator MoveToDestinationCell(IFieldController field, Point destination)
        {
            // If we didn't adjust the destination, or SPD == 0, or 
            // we ended up with the same X, no actual move is needed.
            if (destination.X == Aggregator.Cell.FieldPosition.X && destination.Y == Aggregator.Cell.FieldPosition.Y)
                yield break;

            // Otherwise, move to the newly computed cell
            var targetCell = field.FieldGrid[destination.Y][destination.X];
            yield return field.MoveCardToCell(Aggregator, targetCell);
        }

        /// <summary>
        /// After movement, handle interactions such as fighting a collided unit 
        /// or passing the home row if we are behind enemy lines.
        /// </summary>
        public IEnumerator HandlePostMovementState(IFieldController field, ICardController collided, Point destination)
        {
            if (collided != null)
            {
                // Fight the unit we collided with
                yield return Aggregator.Fight(collided);
            }
            else
            {
                // If no collision, check if we ended up behind enemy lines
                var destinationCell = field.FieldGrid[destination.Y][destination.X];
                if (destinationCell.Player != Owner && destinationCell.Player != Player.Neutral)
                {
                    // Pass the home row if allowed
                    yield return Aggregator.PassHomeRow();
                }
            }
        }

        /// <summary>
        /// A card attacks the opponent's home row, dealing damage to the opponent and moving to the deck.
        /// </summary>
        /// <returns></returns>
        public IEnumerator PassHomeRow()
        {
            if (CanPassHomeRow)
            {
                yield return Aggregator.HealthManager.ModifyPlayerHealth(Aggregator.Owner ==
                   Player.Player1 ? Player.Player2 : Player.Player1, -Aggregator.StatBlock.Stats.ATK);
                yield return Aggregator.ZoneManager.MoveCardFromFieldToDeck(Aggregator, Aggregator.Owner, 0, true);
            }
        }

        public string GetCostText()
        {
            return _view.GetCostText();
        }

        public IEnumerator MoveToCell(ICellController cell, float moveCardTime)
        {
            yield return _view.MoveToCell(cell, moveCardTime);
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
            if (LastManas != null)
            {
                _view.UpdateCostTextWithManaTypes(LastManas, Aggregator.TotalCosts, Aggregator.DeterminePlayability(), Aggregator.Zone is IHandController);
            }
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

        public bool IsCellAvailable(ICellController cell)
        {
            if (cell.CardsOnCell == 1)
            {
               var occupant = cell.GetCardAtIndex(0);
                if (occupant.RequestHasStatusEffect<StatusEffect_Bond>())
                {
                    return occupant.Owner == Aggregator.Owner;
                }
            }
            return cell.CardsOnCell == 0 && cell.Player == Aggregator.Owner;
        }
        #endregion
    }
}
