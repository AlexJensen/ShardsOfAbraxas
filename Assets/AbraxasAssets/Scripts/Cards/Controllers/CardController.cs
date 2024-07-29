using Abraxas.Cards.Data;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.Cells.Controllers;
using Abraxas.Core;
using Abraxas.Events;
using Abraxas.Events.Managers;
using Abraxas.GameStates;
using Abraxas.Health.Managers;
using Abraxas.Players.Managers;
using Abraxas.StatBlocks.Controllers;
using Abraxas.StatBlocks.Data;
using Abraxas.StatusEffects;
using Abraxas.StatusEffects.Types;
using Abraxas.Stones;
using Abraxas.Stones.Controllers;
using Abraxas.Unity.Interfaces;
using Abraxas.Zones.Controllers;
using Abraxas.Zones.Fields.Controllers;
using Abraxas.Zones.Fields.Managers;
using Abraxas.Zones.Hands.Controllers;
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
    /// CardController facilitates communication between the card model, card view, all managers, and any other controllers.
    /// </summary>
    class CardController : ICardController, IGameEventListener<Event_ManaModified>,
                                            IGameEventListener<Event_CardChangedZones>,
                                            IGameEventListener<Event_GameStateEntered>,
                                            IGameEventListener<Event_ActivePlayerChanged>
    {
        #region Dependencies
        ICardModel _model;
        ICardView _view;

        readonly IPlayerManager _playerManager;
        readonly IGameStateManager _gameStateManager;
        readonly IZoneManager _zoneManager;
        readonly IEventManager _eventManager;
        readonly IPlayerHealthManager _healthManager;
        readonly IFieldManager _fieldManager;

        public CardController(IPlayerManager playerManager, IGameStateManager gameStateManager, IZoneManager zoneManager, IEventManager eventManager, IPlayerHealthManager healthManager, IFieldManager fieldManager)
        {
            _playerManager = playerManager;
            _gameStateManager = gameStateManager;
            _zoneManager = zoneManager;
            _eventManager = eventManager;
            _healthManager = healthManager;
            _fieldManager = fieldManager;
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

        public class Factory : PlaceholderFactory<CardData, ICardController>
        {

        }
        #endregion

        #region Fields
        List<Manas.ManaType> _lastManas = new();
        List<IStatusEffect> _activeStatusEffects = new();
        List<IStatusEffect> _markedForRemoval = new();
        List<CardControllerDecorator> _decorators = new();
        #endregion

        #region Properties
        public IStatBlockController StatBlock => _model.StatBlock;
        public List<IStoneController> Stones => _model.Stones;
        public string Title { get => _model.Title; set => _model.Title = value; }
        public Player Owner { get => _model.Owner; set => _model.Owner = value; }
        public Player OriginalOwner { get => _model.OriginalOwner; set => _model.OriginalOwner = value; }

        public Dictionary<StoneType, int> TotalCosts { get => _model.TotalCosts; }
        public ICellController Cell { get => _model.Cell; set => _model.Cell = value; }
        public IZoneController Zone
        {
            get => _model.Zone; 
            set
            {
                if (_model.Zone != value) PreviousZone = Zone;
                _model.Zone = value;
            }
        }
        public IZoneController PreviousZone { get; set; }

        public bool Hidden { get => _model.Hidden; set => _model.Hidden = value; }
        public ITransformManipulator TransformManipulator => (ITransformManipulator)_view;

        public IImageManipulator ImageManipulator => (IImageManipulator)_view;
        public RectTransformMover RectTransformMover => _view.RectTransformMover;
        #endregion

        #region Methods
        public IEnumerator PassHomeRow()
        {
            _healthManager.ModifyPlayerHealth(Owner ==
                Player.Player1 ? Player.Player2 : Player.Player1,

                -StatBlock.Stats.ATK);

            yield return _zoneManager.MoveCardFromFieldToDeck(this, Owner, 0, true);
        }

        public IEnumerator Fight(ICardController opponent)
        {
            if (opponent.Owner == Owner) yield break;

            IStatBlockController collided = opponent.StatBlock;

            StatData collidedStats = collided.Stats;
            StatData stats = StatBlock.Stats;

            collidedStats.DEF -= StatBlock.Stats.ATK;
            stats.DEF -= collidedStats.ATK;

            collided.Stats = collidedStats;
            StatBlock.Stats = stats;


            yield return Utilities.WaitForCoroutines(
                opponent.CheckDeath(),
                CheckDeath());
        }

        public void ApplyStatusEffect(IStatusEffect effect)
        {
            _activeStatusEffects.Add(effect);
            effect.ApplyEffect(this);

            var decorator = (CardControllerDecorator)effect.GetDecorator(this);
            if (decorator != null)
            {
                _decorators.Add(decorator);
            }
        }

        public bool HasStatusEffect<T>() where T : IStatusEffect
        {
            return _activeStatusEffects.Any(e => e is T);
        }

        public void RemoveStatusEffect<T>() where T : IStatusEffect
        {
            _activeStatusEffects.RemoveAll(e => e is T);
            _decorators.RemoveAll(d => d.GetType() == typeof(T).GetMethod("GetDecorator").ReturnType);
        }

        public void MarkStatusEffectForRemoval<T>() where T : IStatusEffect
        {
            var effect = _activeStatusEffects.FirstOrDefault(e => e is T);
            if (effect != null)
            {
                _markedForRemoval.Add(effect);
            }
        }

        public IEnumerator RangedAttack(ICardController opponent)
        {
            if (opponent.Owner == Owner) yield break;

            IStatBlockController collided = opponent.StatBlock;

            StatData collidedStats = collided.Stats;

            collidedStats.DEF -= StatBlock.Stats.ATK;

            collided.Stats = collidedStats;

            yield return Utilities.WaitForCoroutines(
                               opponent.CheckDeath());
        }

        public IEnumerator CheckDeath()
        {

            if (StatBlock.Stats.DEF > 0 || Zone is not IFieldController) yield break;

            yield return _zoneManager.MoveCardFromFieldToGraveyard(this, Owner);
        }

        public IEnumerator Combat()
        {
            bool shouldContinue = true;

            foreach (var decorator in _decorators)
            {
                var enumerator = decorator.Combat();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current is bool result && !result)
                    {
                        shouldContinue = false;
                        break;
                    }
                    yield return enumerator.Current;
                }

                if (!shouldContinue)
                {
                    break;
                }
            }

            // Remove marked status effects after the loop
            foreach (var effect in _markedForRemoval)
            {
                _activeStatusEffects.Remove(effect);
                _decorators.RemoveAll(d => d.GetType() == effect.GetDecorator(this).GetType());
            }
            _markedForRemoval.Clear();

            if (shouldContinue)
            {
                yield return _fieldManager.CombatMovement(this, new Point(
                    Owner == Player.Player1 ? StatBlock.Stats.SPD :
                    Owner == Player.Player2 ? -StatBlock.Stats.SPD : 0, 0));
            }
        }

        public void ChangeScale(PointF pointF, float scaleCardToOverlayTime)
        {
            _view.ChangeScale(pointF, scaleCardToOverlayTime);
        }

        public void SetToInitialScale()
        {
            _view.SetToInitialScale();
        }

        public void SetCardPositionToMousePosition()
        {
            _view.SetCardPositionToMousePosition();
        }

        public string GetCostText()
        {
            return _view.GetCostText();
        }

        public IEnumerator MoveToCell(ICellController cell, float moveCardTime)
        {
            yield return _view.MoveToCell(cell, moveCardTime);
        }

        public void UpdatePlayabilityAndCostText()
        {
            bool isPlayable = DeterminePlayability();
            _view.UpdateCostTextWithManaTypes(_lastManas, TotalCosts, isPlayable);
        }

        public bool DeterminePlayability()
        {
            if (Zone is not IHandController ||
                !(_gameStateManager.State is BeforeCombatState or AfterCombatState) ||
                _playerManager.ActivePlayer != Owner)
            {
                return false;
            }

            foreach (var _ in from pair in TotalCosts
                              let manaPair = _lastManas.FirstOrDefault(m => m.Type == pair.Key)
                              where manaPair == null || pair.Value > manaPair.Amount
                              select new { })
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Delegate Methods
        public IEnumerator OnEventRaised(Event_ManaModified eventData)
        {
            _lastManas = eventData.Data.ManaTypes;
            UpdatePlayabilityAndCostText();
            yield break;
        }

        public bool ShouldReceiveEvent(Event_ManaModified eventData)
        {
            return eventData.Data.Player == Owner && eventData.Data.ManaTypes != null;
        }

        public IEnumerator OnEventRaised(Event_CardChangedZones eventData)
        {
            if (Zone is IFieldController)
            {
                var summoningSickness = new StatusEffect_SummoningSickness();
                ApplyStatusEffect(summoningSickness);
            }
            if (_lastManas != null)
            {
                UpdatePlayabilityAndCostText();
            }
            yield break;
        }

        public bool ShouldReceiveEvent(Event_CardChangedZones eventData)
        {
            return eventData.Data == this;
        }

        public IEnumerator OnEventRaised(Event_GameStateEntered eventData)
        {
            if (_gameStateManager.State is EndState)
            {
                RemoveStatusEffect<StatusEffect_SummoningSickness>();
            }

            if (_lastManas != null)
            {
                UpdatePlayabilityAndCostText();
            }
            yield break;
        }

        public bool ShouldReceiveEvent(Event_GameStateEntered eventData)
        {
            return true;
        }

        public IEnumerator OnEventRaised(Event_ActivePlayerChanged eventData)
        {
            if (_lastManas != null)
            {
                UpdatePlayabilityAndCostText();
            }
            yield break;
        }

        public bool ShouldReceiveEvent(Event_ActivePlayerChanged eventData)
        {
            return true;
        }
        #endregion
    }
}