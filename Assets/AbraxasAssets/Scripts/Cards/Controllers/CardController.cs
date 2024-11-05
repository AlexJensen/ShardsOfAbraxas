using Abraxas.Cards.Data;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.Cells.Controllers;
using Abraxas.Events;
using Abraxas.Events.Managers;
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
using System.Linq;
using Zenject;
using Player = Abraxas.Players.Players;

namespace Abraxas.Cards.Controllers
{
    /// <summary>
    /// CardController is a controller-level class for handling card events. It integrates with a decorator pattern to apply status effects to cards and operates as the central line of communication between the card model and view and other  controllers as well as managers.
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

        public CardController(DiContainer container, IEventManager eventManager)
        {
            _container = container;
            _eventManager = eventManager;
        }

        public void Initialize(ICardModel model, ICardView view)
        {
            _model = model;
            _view = view;

            _decorator = _container.Instantiate<CardControllerDecorator>(new object[] { this, _model, _view });

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
            // Remove listeners from old decorator
            _decorator.OnDestroy();

            // Start with the base decorator
            _decorator = _container.Instantiate<CardControllerDecorator>(new object[] { this, _model, _view });

            // Apply decorators for the active status effects
            foreach (var effect in _activeStatusEffects)
            {
                var newDecorator = effect.GetDecorator(_decorator, _model, _view, _container);
                if (newDecorator != null)
                {
                    _decorator = (ICardControllerInternal)newDecorator;
                }
            }
        }

        public void RequestApplyStatusEffect(IStatusEffect effect) => _decorator.RequestApplyStatusEffect(effect);
        public bool RequestHasStatusEffect<T>() where T : IStatusEffect => _decorator.RequestHasStatusEffect<T>();
        public void RequestRemoveStatusEffect<T>() where T : IStatusEffect => _decorator.RequestRemoveStatusEffect<T>();
        public IEnumerator PassHomeRow() => _decorator.PassHomeRow();
        public IEnumerator Fight(ICardController opponent) => _decorator.Fight(opponent);
        public IEnumerator Attack(ICardController opponent) => _decorator.Attack(opponent);
        public IEnumerator CheckDeath() => _decorator.CheckDeath();
        public IEnumerator Combat(IFieldController field) => _decorator.Combat(field);
        public void ChangeScale(PointF pointF, float scaleCardToOverlayTime) => _decorator.ChangeScale(pointF, scaleCardToOverlayTime);
        public void SetToInitialScale() => _decorator.SetToInitialScale();
        public void SetCardPositionToMousePosition() => _decorator.SetCardPositionToMousePosition();
        public string GetCostText() => _decorator.GetCostText();
        public IEnumerator MoveToCell(ICellController cell, float moveCardTime) => _decorator.MoveToCell(cell, moveCardTime);
        public void UpdatePlayabilityAndCostText() => _decorator.UpdatePlayabilityAndCostText();
        public bool DeterminePlayability() => _decorator.DeterminePlayability();
        public IEnumerator PlayAnimationClip(UnityEngine.AnimationClip clip, UnityEngine.Color color, bool flip) => _decorator.PlayAnimationClip(clip, color, flip);
        #endregion
    }
}
