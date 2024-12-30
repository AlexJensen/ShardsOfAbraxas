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

        #region Fields
        
        
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
        public bool HasAttacked { get => InnerController.HasAttacked; set => _innerController.HasAttacked = value; }
        #endregion

        #region Flags
        public bool EnablePreMovementRangedAttack { get => InnerController.EnablePreMovementRangedAttack; set => InnerController.EnablePreMovementRangedAttack = value; }
        public bool EnablePostMovementRangedAttack { get => InnerController.EnablePostMovementRangedAttack; set => InnerController.EnablePostMovementRangedAttack = value; }
        public bool CanFight { get => _innerController.CanFight; set => _innerController.CanFight = value; }
        public virtual bool CanBeAttackedRanged { get => _innerController.CanBeAttackedRanged; set => _innerController.CanBeAttackedRanged = value; }
        #endregion

        #region Methods
        public virtual void RequestApplyStatusEffect(IStatusEffect effect)
        {
            ApplyStatusEffect(effect);
        }

        public virtual bool RequestHasStatusEffect<T>() where T : IStatusEffect
        {
            return HasStatusEffect<T>();
        }

        public virtual void RequestRemoveStatusEffect<T>() where T : IStatusEffect
        {
            RemoveStatusEffect<T>();
        }

        public virtual void ApplyStatusEffect(IStatusEffect effect) => InnerController.ApplyStatusEffect(effect);
        public virtual bool HasStatusEffect<T>() where T : IStatusEffect => InnerController.HasStatusEffect<T>();
        public virtual void RemoveStatusEffect<T>() where T : IStatusEffect => InnerController.RemoveStatusEffect<T>();

        /// <summary>
        /// A card attacks the opponent's home row, dealing damage to the opponent and moving to the deck.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator PassHomeRow()
        {
            yield return InnerController.PassHomeRow();
        }

        /// <summary>
        /// A card fights an opponent's card, each dealing damage to the other equal to its ATK value.
        /// </summary>
        /// <param name="opponent">Card to fight</param>
        /// <returns></returns>
        public virtual IEnumerator Fight(ICardController opponent)
        {
            yield return InnerController.Fight(opponent);
        }

        /// <summary>
        /// A card attacks an opponent's card. Normally cards can only do this once per combat step, ranged or melee.
        /// </summary>
        /// <param name="opponent"></param>
        /// <returns></returns>
        public virtual IEnumerator Attack(ICardController opponent, bool ranged)
        {
            yield return InnerController.Attack(opponent, ranged);
        }

        /// <summary>
        /// A card deals damage to an opponent's card.
        /// </summary>
        /// <param name="opponent"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public virtual IEnumerator DealDamage(ICardController opponent, int amount)
        {
            yield return InnerController.DealDamage(opponent, amount);
        }

        /// <summary>
        /// A card takes damage from a source.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public virtual IEnumerator TakeDamage(ICardController source, int amount)
        {
            yield return InnerController.TakeDamage(source, amount);
        }

        /// <summary>
        /// A card is destroyed if its DEF is reduced to 0 or less while on the field. 
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator CheckDeath()
        {

            yield return InnerController.CheckDeath();
        }

        public virtual IEnumerator PreCombat()
        {
            return InnerController.PreCombat();
        }

        /// <summary>
        /// Combat is split into three phases: pre-movement, movement, and post-movement. Each phase can have different actions and effects applied to the card.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public virtual IEnumerator Combat(IFieldController field)
        {
            yield return InnerController.Combat(field);
        }

        /// <summary>
        /// Pre-movement actions are applied to the card before it moves on the field.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public virtual IEnumerator PreMovementAction(IFieldController field)
        {
            yield return InnerController.PreMovementAction(field);
        }

        /// <summary>
        /// A card moves forward on the field equal to its SPD value during combat and will fight enemy cards it comes in contact with during the movement.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator MoveAndHandleCollisions(IFieldController field)
        {
           yield return InnerController.MoveAndHandleCollisions(field);
        }

        /// <summary>
        /// After movement actions are applied to the card after it moves on the field.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public virtual IEnumerator PostMovementAction(IFieldController field)
        {
            yield return InnerController.PostMovementAction(field);
        }

        /// <summary>
        /// Performs a ranged attack if able.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="doAttack"></param>
        /// <returns></returns>
        public virtual IEnumerator RangedAttack(IFieldController field, bool doAttack)
        {
            yield return InnerController.RangedAttack(field, doAttack);
        }

        /// <summary>
        /// Ranged attack is a special attack that allows a card to attack an opponent's card from a distance. The card will attack the first enemy card it encounters within its range.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="movement"></param>
        /// <returns></returns>
        public virtual ICardController CheckRangedAttack(IFieldController field, Point movement)
        {
            return InnerController.CheckRangedAttack(field, movement);
        }

        /// <summary>
        /// A card can only be played if the player is the active player, the game state is either of the two main phases, the card is in the player's hand, and the player has enough mana to pay the card's cost. 
        /// Note this only checks if this specific card can be played, another check to see if there are any available open cells must also be made by the field manager to conclusively determine if the card can be played.
        /// </summary>
        /// <returns></returns>
        public virtual bool DeterminePlayability() => InnerController.DeterminePlayability();

        public virtual void ChangeScale(PointF pointF, float scaleCardToOverlayTime) => InnerController.ChangeScale(pointF, scaleCardToOverlayTime);
        public virtual void SetToInitialScale() => InnerController.SetToInitialScale();
        public virtual void SetCardPositionToMousePosition() => InnerController.SetCardPositionToMousePosition();
        public virtual string GetCostText() => InnerController.GetCostText();

        public virtual IEnumerator PlayAnimationClip(UnityEngine.AnimationClip clip, UnityEngine.Color color, bool flip) => InnerController.PlayAnimationClip(clip, color, flip);

        
        public virtual IEnumerator MoveToCell(ICellController cell, float moveCardTime) => InnerController.MoveToCell(cell, moveCardTime);

        public virtual void UpdatePlayabilityAndCostText() => InnerController.UpdatePlayabilityAndCostText();
        #endregion

        #region Delegate Methods
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
        #endregion
    }
}