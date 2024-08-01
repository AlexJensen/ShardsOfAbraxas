using Abraxas.Cards.Data;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.Cells.Controllers;
using Abraxas.Events;
using Abraxas.Events.Managers;
using Abraxas.GameStates;
using Abraxas.Health.Managers;
using Abraxas.Manas;
using Abraxas.Players.Managers;
using Abraxas.StatBlocks.Controllers;
using Abraxas.StatusEffects;
using Abraxas.StatusEffects.Types;
using Abraxas.Stones;
using Abraxas.Stones.Controllers;
using Abraxas.Unity.Interfaces;
using Abraxas.Zones.Controllers;
using Abraxas.Zones.Fields.Controllers;
using Abraxas.Zones.Fields.Managers;
using Abraxas.Zones.Managers;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Zenject;
using Player = Abraxas.Players.Players;

namespace Abraxas.Cards.Controllers
{
    class CardController : ICardController, IGameEventListener<Event_ManaModified>,
                                            IGameEventListener<Event_CardChangedZones>,
                                            IGameEventListener<Event_GameStateEntered>,
                                            IGameEventListener<Event_ActivePlayerChanged>
    {
        #region Dependencies
        ICardModel _model;
        ICardView _view;

        protected readonly IPlayerManager _playerManager;
        protected readonly IGameStateManager _gameStateManager;
        protected readonly IZoneManager _zoneManager;
        protected readonly IEventManager _eventManager;
        protected readonly IPlayerHealthManager _healthManager;
        protected readonly IFieldManager _fieldManager;
        protected readonly DiContainer _container;

        public CardController(IPlayerManager playerManager, IGameStateManager gameStateManager, IZoneManager zoneManager, IEventManager eventManager, IPlayerHealthManager healthManager, IFieldManager fieldManager, DiContainer container)
        {
            _playerManager = playerManager;
            _gameStateManager = gameStateManager;
            _zoneManager = zoneManager;
            _eventManager = eventManager;
            _healthManager = healthManager;
            _fieldManager = fieldManager;
            _container = container;

            // Initialize with the base decorator that directly interacts with the model and view
            _decorator = _container.Instantiate<CardControllerDecorator>(new object[] { this, _model, _view });
        }

        public void Initialize(ICardModel model, ICardView view)
        {
            _model = model;
            _view = view;

            _eventManager.AddListener(typeof(Event_ManaModified), this as IGameEventListener<Event_ManaModified>);
            _eventManager.AddListener(typeof(Event_CardChangedZones), this as IGameEventListener<Event_CardChangedZones>);
            _eventManager.AddListener(typeof(Event_GameStateEntered), this as IGameEventListener<Event_GameStateEntered>);
            _eventManager.AddListener(typeof(Event_ActivePlayerChanged), this as IGameEventListener<Event_ActivePlayerChanged>);
        }

        public void OnDestroy()
        {
            _eventManager.RemoveListener(typeof(Event_ManaModified), this as IGameEventListener<Event_ManaModified>);
            _eventManager.RemoveListener(typeof(Event_CardChangedZones), this as IGameEventListener<Event_CardChangedZones>);
            _eventManager.RemoveListener(typeof(Event_GameStateEntered), this as IGameEventListener<Event_GameStateEntered>);
            _eventManager.RemoveListener(typeof(Event_ActivePlayerChanged), this as IGameEventListener<Event_ActivePlayerChanged>);
        }

        public class Factory : PlaceholderFactory<CardData, ICardController> { }

        #endregion

        #region Fields
        List<Manas.ManaType> _lastManas = new();
        readonly List<IStatusEffect> _activeStatusEffects = new();
        ICardController _decorator;
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

        public List<ManaType> LastManas { get => _lastManas; set => _lastManas = value; }
        #endregion

        #region Methods

        public void ApplyStatusEffect(IStatusEffect effect)
        {
            _activeStatusEffects.Add(effect);
            effect.ApplyEffect(this);

            var newDecorator = effect.GetDecorator(_decorator, _model, _view, _container);
            if (newDecorator != null)
            {
                _decorator = newDecorator;
            }
        }

        public bool HasStatusEffect<T>() where T : IStatusEffect => _activeStatusEffects.Any(e => e is T);

        public void RemoveStatusEffect<T>() where T : IStatusEffect
        {
            _activeStatusEffects.RemoveAll(e => e is T);

            // Reset the decorator chain
            _decorator = _container.Instantiate<CardControllerDecorator>(new object[] { this, _model, _view });

            // Reapply decorators for the remaining status effects
            foreach (var effect in _activeStatusEffects)
            {
                _decorator = effect.GetDecorator(_decorator, _model, _view, _container);
            }
        }

        public IEnumerator PassHomeRow() => _decorator.PassHomeRow();
        public IEnumerator Fight(ICardController opponent) => _decorator.Fight(opponent);
        public IEnumerator RangedAttack(ICardController opponent) => _decorator.RangedAttack(opponent);
        public IEnumerator CheckDeath() => _decorator.CheckDeath();
        public IEnumerator Combat() => _decorator.Combat();
        public void ChangeScale(PointF pointF, float scaleCardToOverlayTime) => _decorator.ChangeScale(pointF, scaleCardToOverlayTime);
        public void SetToInitialScale() => _decorator.SetToInitialScale();
        public void SetCardPositionToMousePosition() => _decorator.SetCardPositionToMousePosition();
        public string GetCostText() => _decorator.GetCostText();
        public IEnumerator MoveToCell(ICellController cell, float moveCardTime) => _decorator.MoveToCell(cell, moveCardTime);
        public void UpdatePlayabilityAndCostText() => _decorator.UpdatePlayabilityAndCostText();
        public bool DeterminePlayability() => _decorator.DeterminePlayability();
        #endregion

        #region Delegate Methods
        public IEnumerator OnEventRaised(Event_ManaModified eventData)
        {
            LastManas = eventData.Data.ManaTypes;
            UpdatePlayabilityAndCostText();
            yield break;
        }

        public bool ShouldReceiveEvent(Event_ManaModified eventData) => eventData.Data.Player == Owner && eventData.Data.ManaTypes != null;

        public IEnumerator OnEventRaised(Event_CardChangedZones eventData)
        {
            if (Zone is IFieldController)
            {
                ApplyStatusEffect(new StatusEffect_SummoningSickness());
            }
            if (LastManas != null)
            {
                UpdatePlayabilityAndCostText();
            }
            yield break;
        }

        public bool ShouldReceiveEvent(Event_CardChangedZones eventData) => eventData.Data == this;

        public IEnumerator OnEventRaised(Event_GameStateEntered eventData)
        {
            if (_gameStateManager.State is EndState)
            {
                RemoveStatusEffect<StatusEffect_SummoningSickness>();
            }

            if (LastManas != null)
            {
                UpdatePlayabilityAndCostText();
            }
            yield break;
        }

        public bool ShouldReceiveEvent(Event_GameStateEntered eventData) => true;

        public IEnumerator OnEventRaised(Event_ActivePlayerChanged eventData)
        {
            if (LastManas != null)
            {
                UpdatePlayabilityAndCostText();
            }
            yield break;
        }

        public bool ShouldReceiveEvent(Event_ActivePlayerChanged eventData) => true;
        #endregion
    }
}
