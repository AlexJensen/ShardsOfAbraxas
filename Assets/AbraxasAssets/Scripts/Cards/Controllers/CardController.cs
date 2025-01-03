using Abraxas.Cards.Data;
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
using Abraxas.Zones.Managers;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Zenject;
using Player = Abraxas.Players.Players;

namespace Abraxas.Cards.Controllers
{
    /// <summary>
    /// CardController is an aggregator class for handling card events. It orchestrates incoming events and function calls to the top level of the decorator chain and serves as the middleman for communication between the card and other cards or manager classes.
    /// </summary>
    class CardController : ICardControllerInternal,
                           IGameEventListener<Event_ManaModified>,
                           IGameEventListener<Event_CardChangedZones>,
                           IGameEventListener<Event_GameStateEntered>,
                           IGameEventListener<Event_ActivePlayerChanged>
    {
        #region Dependencies
        ICardModel _model;
        ICardView _view;
        ICardControllerInternal _decorator;
        readonly DiContainer _container;
        readonly IEventManager _eventManager;

        public IGameStateManager GameStateManager { get; private set; }

        public IPlayerManager PlayerManager { get; private set; }

        public IZoneManager ZoneManager { get; private set; }

        public IPlayerHealthManager HealthManager { get; private set; }

        public CardController(DiContainer container, IEventManager eventManager)
        {
            _container = container;
            _eventManager = eventManager;
        }

        [Inject]
        public void Construct(IGameStateManager gameState, IPlayerHealthManager healthManager, IZoneManager zoneManager, IPlayerManager playerManager)
        {
            GameStateManager = gameState;
            HealthManager = healthManager;
            ZoneManager = zoneManager;
            PlayerManager = playerManager;
        }

        public void Initialize(ICardModel model, ICardView view)
        {
            _model = model;
            _view = view;

            var baseController = new BaseCardController(this, _model, _view);
            _decorator = _container.Instantiate<CardDecorator>(
                new object[] { baseController, _model, _view });

            // Register this as the event listener
            InitializeListeners();
        }


        public void OnDestroy()
        {
            RemoveListeners();
            _decorator.OnDestroy();
        }

        public void InitializeListeners()
        {
            _eventManager.AddListener(this as IGameEventListener<Event_ManaModified>);
            _eventManager.AddListener(this as IGameEventListener<Event_CardChangedZones>);
            _eventManager.AddListener(this as IGameEventListener<Event_GameStateEntered>);
            _eventManager.AddListener(this as IGameEventListener<Event_ActivePlayerChanged>);
        }

        public void RemoveListeners()
        {
            _eventManager.RemoveListener(this as IGameEventListener<Event_ManaModified>);
            _eventManager.RemoveListener(this as IGameEventListener<Event_CardChangedZones>);
            _eventManager.RemoveListener(this as IGameEventListener<Event_GameStateEntered>);
            _eventManager.RemoveListener(this as IGameEventListener<Event_ActivePlayerChanged>);
        }

        public class Factory : PlaceholderFactory<CardData, ICardController> { }
        #endregion

        #region Event Handlers
        public IEnumerator OnEventRaised(Event_ManaModified eventData) => _decorator.OnEventRaised(eventData);
        public bool ShouldReceiveEvent(Event_ManaModified eventData) => _decorator.ShouldReceiveEvent(eventData);
        public IEnumerator OnEventRaised(Event_CardChangedZones eventData) => _decorator.OnEventRaised(eventData);
        public bool ShouldReceiveEvent(Event_CardChangedZones eventData) => _decorator.ShouldReceiveEvent(eventData);
        public IEnumerator OnEventRaised(Event_GameStateEntered eventData) => _decorator.OnEventRaised(eventData);
        public bool ShouldReceiveEvent(Event_GameStateEntered eventData) => _decorator.ShouldReceiveEvent(eventData);
        public IEnumerator OnEventRaised(Event_ActivePlayerChanged eventData) => _decorator.OnEventRaised(eventData);
        public bool ShouldReceiveEvent(Event_ActivePlayerChanged eventData) => _decorator.ShouldReceiveEvent(eventData);
        #endregion

        #region Fields
        readonly List<IStatusEffect> _activeStatusEffects = new();
        #endregion

        #region Properties
        public IStatBlockController StatBlock => _decorator.StatBlock;
        public List<IStoneController> Stones => _decorator.Stones;
        public string Title { get => _decorator.Title; set => _decorator.Title = value; }
        public Player Owner { get => _decorator.Owner; set => _decorator.Owner = value; }
        public Player OriginalOwner { get => _decorator.OriginalOwner; set => _decorator.OriginalOwner = value; }
        public Dictionary<StoneType, int> TotalCosts => _decorator.TotalCosts;
        public ICellController Cell { get => _decorator.Cell; set => _decorator.Cell = value; }
        public IZoneController Zone
        {
            get => _decorator.Zone;
            set
            {
                if (_decorator.Zone != value) PreviousZone = Zone;
                _decorator.Zone = value;
            }
        }
        public IZoneController PreviousZone { get; set; }
        public bool Hidden { get => _decorator.Hidden; set => _decorator.Hidden = value; }
        public ITransformManipulator TransformManipulator => _decorator.TransformManipulator;
        public IImageManipulator ImageManipulator => _decorator.ImageManipulator;
        public RectTransformMover RectTransformMover => _decorator.RectTransformMover;

        public ICardControllerInternal Aggregator => this;

        #endregion

        #region Flags
        public bool EnablePreMovementRangedAttack { get; set; }
        public bool EnablePostMovementRangedAttack { get; set; }
        public bool HasAttacked { get; set; }
        public bool CanFight { get; set; }
        public bool CanPassHomeRow { get; set; }
        public bool CanBeAttackedRanged { get; set; }
        public bool CanAlliedRangedAttacksShootThrough { get; set; }
        #endregion

        #region Methods
        public void ApplyStatusEffect(IStatusEffect effect)
        {
            _activeStatusEffects.Add(effect);
            effect.ApplyEffect(this);

            // Rebuild the decorator chain
            RebuildDecoratorChain();
        }

        public bool HasStatusEffect<T>() where T : IStatusEffect => _activeStatusEffects.Any(e => e is T);

        public void RemoveStatusEffect<T>() where T : IStatusEffect
        {
            _activeStatusEffects.RemoveAll(e => e is T);

            // Rebuild the decorator chain
            RebuildDecoratorChain();
        }

        private void RebuildDecoratorChain()
        {
            // Start from a fresh BaseCardController
            var baseController = _container.Instantiate<BaseCardController>(new object[] { this, _model, _view });
            ICardControllerInternal chain = baseController;

            // Apply each status effect as a decorator
            foreach (var effect in _activeStatusEffects)
            {
                if (effect.GetDecorator(chain, _model, _view, _container) is ICardControllerInternal newDecorator)
                {
                    chain = newDecorator;
                }
            }
            _decorator = chain;
        }

        public void RequestApplyStatusEffect(IStatusEffect effect) => _decorator.RequestApplyStatusEffect(effect);
        public bool RequestHasStatusEffect<T>() where T : IStatusEffect => _decorator.RequestHasStatusEffect<T>();
        public void RequestRemoveStatusEffect<T>() where T : IStatusEffect => _decorator.RequestRemoveStatusEffect<T>();
        public IEnumerator PassHomeRow() => _decorator.PassHomeRow();
        public IEnumerator Fight(ICardController opponent) => _decorator.Fight(opponent);
        public IEnumerator Attack(ICardController opponent, bool ranged) => _decorator.Attack(opponent, ranged);
        public IEnumerator CheckDeath() => _decorator.CheckDeath();
        public IEnumerator PreCombat() => _decorator.PreCombat();
        public IEnumerator Combat(IFieldController field) => _decorator.Combat(field);
        public void ChangeScale(PointF pointF, float scaleCardToOverlayTime) => _decorator.ChangeScale(pointF, scaleCardToOverlayTime);
        public void SetToInitialScale() => _decorator.SetToInitialScale();
        public void SetCardPositionToMousePosition() => _decorator.SetCardPositionToMousePosition();
        public string GetCostText() => _decorator.GetCostText();
        public IEnumerator MoveToCell(ICellController cell, float moveCardTime) => _decorator.MoveToCell(cell, moveCardTime);
        public void UpdatePlayabilityAndCostText() => _decorator.UpdatePlayabilityAndCostText();
        public bool DeterminePlayability() => _decorator.DeterminePlayability();
        public IEnumerator PlayAnimationClip(UnityEngine.AnimationClip clip, UnityEngine.Color color, bool flip) => _decorator.PlayAnimationClip(clip, color, flip);
       
        public IEnumerator MoveAndHandleCollisions(IFieldController field) => _decorator.MoveAndHandleCollisions(field);
        public IEnumerator DealDamage(ICardController opponent, int amount) => _decorator.DealDamage(opponent, amount);
        public IEnumerator TakeDamage(ICardController source, int amount) => _decorator.TakeDamage(source, amount);

        public IEnumerator PreMovementAction(IFieldController field)
        {
            return _decorator.PreMovementAction(field);
        }

        public IEnumerator PostMovementAction(IFieldController field)
        {
            return _decorator.PostMovementAction(field);
        }

        public IEnumerator RangedAttack(IFieldController field, bool doAttack)
        {
            return _decorator.RangedAttack(field, doAttack);
        }

        public ICardController CheckRangedAttack(IFieldController field, Point movement)
        {
            return _decorator.CheckRangedAttack(field, movement);
        }

        public Point CalculateDestination(IFieldController field, Point movement)
        {
            return _decorator.CalculateDestination(field, movement);
        }

        public ICardController FindCollisionAlongPath(IFieldController field, ref Point destination, Point movement)
        {
            return _decorator.FindCollisionAlongPath(field, ref destination, movement);
        }

        public Point GetMovementVector()
        {
            return _decorator.GetMovementVector();
        }

        public IEnumerator HandlePostMovementState(IFieldController field, ICardController collided, Point destination)
        {
            return _decorator.HandlePostMovementState(field, collided, destination);
        }

        public IEnumerator MoveToDestinationCell(IFieldController field, Point destination)
        {
            return _decorator.MoveToDestinationCell(field, destination);
        }

        public bool IsCellAvailable(ICellController cell)
        {
            return _decorator.IsCellAvailable(cell);
        }
        #endregion
    }
}
