﻿using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.Cells.Controllers;
using Abraxas.Core;
using Abraxas.Events;
using Abraxas.Events.Managers;
using Abraxas.GameStates;
using Abraxas.Health.Managers;
using Abraxas.Manas;
using Abraxas.Players.Managers;
using Abraxas.StatBlocks.Controllers;
using Abraxas.StatBlocks.Data;
using Abraxas.StatusEffects;
using Abraxas.StatusEffects.Types;
using Abraxas.Stones;
using Abraxas.Stones.Controllers;
using Abraxas.UI;
using Abraxas.Unity.Interfaces;
using Abraxas.Zones.Controllers;
using Abraxas.Zones.Fields.Controllers;
using Abraxas.Zones.Fields.Managers;
using Abraxas.Zones.Hands.Controllers;
using Abraxas.Zones.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Zenject;
using Player = Abraxas.Players.Players;

namespace Abraxas.Cards.Controllers
{
    /// <summary>
    /// The default CardControllerDecorator class functions as the baseline for expected card behaviour and operates as the root of the decorator pattern.
    /// </summary>
    class CardControllerDecorator : ICardControllerInternal
                                    
    {
        #region Dependencies
        protected ICardControllerInternal _innerController;
        protected ICardModel _model;
        protected ICardView _view;

        protected IPlayerManager _playerManager;
        protected IGameStateManager _gameStateManager;
        protected IZoneManager _zoneManager;
        protected IEventManager _eventManager;
        protected IPlayerHealthManager _healthManager;
        protected IFieldManager _fieldManager;

        public CardControllerDecorator(ICardControllerInternal innerController, ICardModel model, ICardView view)
        {
            _innerController = innerController;
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

        public virtual void OnDestroy()
        {

        }
        #endregion

        #region Fields
        List<Manas.ManaType> _lastManas = new();
        #endregion

        #region Properties
        public virtual IStatBlockController StatBlock => _model.StatBlock;
        public virtual List<IStoneController> Stones => _model.Stones;
        public virtual string Title { get => _model.Title; set => _model.Title = value; }
        public virtual Player Owner { get => _model.Owner; set => _model.Owner = value; }
        public virtual Player OriginalOwner { get => _model.OriginalOwner; set => _model.OriginalOwner = value; }
        public virtual Dictionary<StoneType, int> TotalCosts => _model.TotalCosts;
        public virtual ICellController Cell { get => _model.Cell; set => _model.Cell = value; }
        public virtual IZoneController Zone { get => _model.Zone; set => _model.Zone = value; }
        public virtual IZoneController PreviousZone { get => InnerController.PreviousZone; set => InnerController.PreviousZone = value; }
        public virtual bool Hidden { get => _model.Hidden; set => _model.Hidden = value; }
        public virtual ITransformManipulator TransformManipulator => (ITransformManipulator)_view;
        public virtual IImageManipulator ImageManipulator => (IImageManipulator)_view;
        public virtual RectTransformMover RectTransformMover => _view.RectTransformMover;
        public List<ManaType> LastManas { get => _lastManas; set => _lastManas = value; }
        public ICardControllerInternal InnerController { get => _innerController; set => _innerController = value; }
        #endregion

        #region Methods
        public ICardController GetBaseCard()
        {
            ICardController card = this;
            while (card is CardControllerDecorator decorator)
            {
                card = decorator.InnerController;
            }
            return card;
        }
        public void RequestApplyStatusEffect(IStatusEffect effect) => ApplyStatusEffect(effect);

        public bool RequestHasStatusEffect<T>() where T : IStatusEffect => HasStatusEffect<T>();

        public void RequestRemoveStatusEffect<T>() where T : IStatusEffect => RemoveStatusEffect<T>();
        public virtual void ApplyStatusEffect(IStatusEffect effect) => InnerController.ApplyStatusEffect(effect);
        public virtual bool HasStatusEffect<T>() where T : IStatusEffect => InnerController.HasStatusEffect<T>();
        public virtual void RemoveStatusEffect<T>() where T : IStatusEffect => InnerController.RemoveStatusEffect<T>();

        /// <summary>
        /// A card attacks the opponent's home row, dealing damage to the opponent and moving to the deck.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator PassHomeRow()
        {
            yield return _healthManager.ModifyPlayerHealth(Owner ==
                Player.Player1 ? Player.Player2 : Player.Player1, -StatBlock.Stats.ATK);

            yield return _zoneManager.MoveCardFromFieldToDeck(GetBaseCard(), Owner, 0, true);
        }

        /// <summary>
        /// A card fights an opponent's card, each dealing damage to the other equal to its ATK value. If a card's DEF is reduced to 0, it is moved to the graveyard.
        /// </summary>
        /// <param name="opponent">Card to fight</param>
        /// <returns></returns>
        public virtual IEnumerator Fight(ICardController opponent)
        {
            if (opponent.Owner == Owner) yield break;

            yield return Utilities.WaitForCoroutines(
                opponent.Attack(this),
                Attack(opponent));
        }

        /// <summary>
        /// A card attacks an opponent's card, dealing damage to the opponent card's DEF equal to the attacker's ATK and moving the opponent's card to the graveyard if its DEF is reduced to 0.
        /// </summary>
        /// <param name="opponent"></param>
        /// <returns></returns>
        public virtual IEnumerator Attack(ICardController opponent)
        {
            IStatBlockController collided = opponent.StatBlock;
            yield return opponent.PlayAnimationClip(_view.Attack, StatBlock.Color, _playerManager.LocalPlayer == Player.Player2);

            StatData collidedStats = collided.Stats;

            collidedStats.DEF -= StatBlock.Stats.ATK;

            collided.Stats = collidedStats;

            yield return Utilities.WaitForCoroutines(
                               opponent.CheckDeath());
        }

        /// <summary>
        /// A card is destroyed if its DEF is reduced to 0 while on the field.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator CheckDeath()
        {

            if (StatBlock.Stats.DEF > 0 || Zone is not IFieldController) yield break;

            yield return _zoneManager.MoveCardFromFieldToGraveyard(GetBaseCard(), Owner);
        }

        /// <summary>
        /// A card moves forward on the field equal to its SPD value during combat and will fight enemy cards it comes in contact with during the movement.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator Combat(IFieldController field)
        {
            if (CanPerformPreMovementAction())
            {
                yield return PreMovementAction(field);
            }

            yield return MoveAndHandleCollisions(field);

            if (CanPerformPostMovementAction())
            {
                yield return PostMovementAction(field);
            }
        }

        protected virtual bool CanPerformPreMovementAction() => false;
        protected virtual bool CanPerformPostMovementAction() => true;

        protected virtual IEnumerator PreMovementAction(IFieldController field)
        {
            // Default: Do nothing before movement
            yield break;
        }

        protected virtual IEnumerator MoveAndHandleCollisions(IFieldController field)
        {
            var movement = new Point(Owner == Player.Player1 ? StatBlock.Stats.SPD : -StatBlock.Stats.SPD, 0);
            Point destination = new(
                Math.Clamp(Cell.FieldPosition.X + movement.X, 0, field.FieldGrid[0].Count - 1),
                Cell.FieldPosition.Y);

            ICardController collided = null;
            var fieldGrid = field.FieldGrid;

            // Movement and collision detection
            for (int i = Cell.FieldPosition.X + Math.Sign(movement.X); i != destination.X + Math.Sign(movement.X); i += Math.Sign(movement.X))
            {
                if (fieldGrid[Cell.FieldPosition.Y][i].CardsOnCell <= 0) continue;
                destination.X = i - Math.Sign(movement.X);
                collided = fieldGrid[Cell.FieldPosition.Y][i].GetCardAtIndex(0);
                break;
            }

            // Move the card if necessary
            if (destination != Cell.FieldPosition)
            {
                yield return field.MoveCardToCell(GetBaseCard(), fieldGrid[destination.Y][destination.X]);
            }

            // Handle collision
            if (collided != null)
            {
                yield return Fight(collided);
            }
            else if (fieldGrid[destination.Y][destination.X].Player != Owner && fieldGrid[destination.Y][destination.X].Player != Player.Neutral)
            {
                yield return PassHomeRow();
            }

            yield break;
        }

        protected virtual IEnumerator PostMovementAction(IFieldController field)
        {
            if (StatBlock.Stats.RNG > 0)
            {
                var target = CheckRangedAttack(field, new Point(Owner == Player.Player1 ? StatBlock.Stats.RNG : -StatBlock.Stats.RNG, 0));
                if (target != null)
                {
                    yield return Attack(target);
                }
            }
            yield break;
        }

        protected virtual ICardController CheckRangedAttack(IFieldController field, Point movement)
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
                return collided;
            }
            return null;
        }

        /// <summary>
        /// A card can only be played if the player is the active player, the game state is either of the two main phases, the card is in the player's hand, and the player has enough mana to pay the card's cost. 
        /// Note this only checks if this specific card can be played, another check to see if there are any available open cells must also be made by the field manager to conclusively determine if the card can be played.
        /// </summary>
        /// <returns></returns>
        public virtual bool DeterminePlayability()
        {
            if (Zone is not IHandController ||
                !(_gameStateManager.State is BeforeCombatState or AfterCombatState) ||
                _playerManager.ActivePlayer != Owner)
            {
                return false;
            }

            foreach (var _ in from pair in TotalCosts
                              let manaPair = LastManas.FirstOrDefault(m => m.Type == pair.Key)
                              where manaPair == null || pair.Value > manaPair.Amount
                              select new { })
            {
                return false;
            }
            return true;
        }

        public virtual void ChangeScale(PointF pointF, float scaleCardToOverlayTime) => _view.ChangeScale(pointF, scaleCardToOverlayTime);
        public virtual void SetToInitialScale() => _view.SetToInitialScale();
        public virtual void SetCardPositionToMousePosition() => _view.SetCardPositionToMousePosition();
        public virtual string GetCostText() => _view.GetCostText();

        public virtual IEnumerator PlayAnimationClip(UnityEngine.AnimationClip clip, UnityEngine.Color color, bool flip)
        {
            yield return _view.PlayAnimation(clip, color, flip);
        }
        public virtual IEnumerator MoveToCell(ICellController cell, float moveCardTime) => _view.MoveToCell(cell, moveCardTime);
       

        public virtual void UpdatePlayabilityAndCostText()
        {
            bool isPlayable = DeterminePlayability();
            _view.UpdateCostTextWithManaTypes(LastManas, TotalCosts, isPlayable, Zone is IHandController);
        }
        #endregion

        #region Delegate Methods
        public virtual IEnumerator OnEventRaised(Event_ManaModified eventData)
        {
            LastManas = eventData.Mana.ManaTypes;
            UpdatePlayabilityAndCostText();
            yield break;
        }

        public virtual bool ShouldReceiveEvent(Event_ManaModified eventData) => eventData.Mana.Player == Owner && eventData.Mana.ManaTypes != null;

        public virtual IEnumerator OnEventRaised(Event_CardChangedZones eventData)
        {
            StatBlock.ShowSymbols = false;
            // Apply summoning sickness when a card enters the field
            if (Zone is IFieldController)
            {
                StatBlock.ShowSymbols = true;
                ApplyStatusEffect(new StatusEffect_SummoningSickness());
            }
            if (LastManas != null)
            {
                UpdatePlayabilityAndCostText();
            }
            yield break;
        }

        public virtual bool ShouldReceiveEvent(Event_CardChangedZones eventData) => eventData.Card.Equals(GetBaseCard());

        public virtual IEnumerator OnEventRaised(Event_GameStateEntered eventData)
        {
            if (LastManas != null)
            {
                UpdatePlayabilityAndCostText();
            }
            yield break;
        }

        public virtual bool ShouldReceiveEvent(Event_GameStateEntered eventData) => true;

        public virtual IEnumerator OnEventRaised(Event_ActivePlayerChanged eventData)
        {
            if (LastManas != null)
            {
                UpdatePlayabilityAndCostText();
            }
            yield break;
        }

        public virtual bool ShouldReceiveEvent(Event_ActivePlayerChanged eventData) => true;

        #endregion
    }
}