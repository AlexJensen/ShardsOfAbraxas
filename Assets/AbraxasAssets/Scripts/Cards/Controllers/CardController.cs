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
using Abraxas.Stones;
using Abraxas.Stones.Controllers;
using Abraxas.Unity.Interfaces;
using Abraxas.Zones.Controllers;
using Abraxas.Zones.Decks.Managers;
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
    class CardController : ICardController, IGameEventListener<ManaModifiedEvent>,
                                            IGameEventListener<CardChangedZonesEvent>,
                                            IGameEventListener<GameStateEnteredEvent>,
                                            IGameEventListener<ActivePlayerChangedEvent>
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
        readonly IDeckManager _deckManager;

        public CardController(IPlayerManager playerManager, IGameStateManager gameStateManager, IZoneManager zoneManager, IDeckManager deckManager, IEventManager eventManager, IPlayerHealthManager healthManager, IFieldManager fieldManager)
        {
            _playerManager = playerManager;
            _gameStateManager = gameStateManager;
            _zoneManager = zoneManager;
            _eventManager = eventManager;
            _healthManager = healthManager;
            _fieldManager = fieldManager;
            _deckManager = deckManager;
        }

        public void Initialize(ICardModel model, ICardView view)
        {
            _model = model;
            _view = view;

            _eventManager.AddListener(typeof(ManaModifiedEvent), this as IGameEventListener<ManaModifiedEvent>);
            _eventManager.AddListener(typeof(CardChangedZonesEvent), this as IGameEventListener<CardChangedZonesEvent>);
            _eventManager.AddListener(typeof(GameStateEnteredEvent), this as IGameEventListener<GameStateEnteredEvent>);
            _eventManager.AddListener(typeof(ActivePlayerChangedEvent), this as IGameEventListener<ActivePlayerChangedEvent>);
        }

        public void OnDestroy()
        {
            _eventManager.RemoveListener(typeof(ManaModifiedEvent), this as IGameEventListener<ManaModifiedEvent>);
            _eventManager.RemoveListener(typeof(CardChangedZonesEvent), this as IGameEventListener<CardChangedZonesEvent>);
            _eventManager.RemoveListener(typeof(GameStateEnteredEvent), this as IGameEventListener<GameStateEnteredEvent>);
            _eventManager.RemoveListener(typeof(ActivePlayerChangedEvent), this as IGameEventListener<ActivePlayerChangedEvent>);
        }

        public class Factory : PlaceholderFactory<CardData, ICardController>
        {

        }
        #endregion

        #region Fields
        List<Manas.ManaType> _lastManas;
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

        public IEnumerator CheckDeath()
        {

            if (StatBlock.Stats.DEF > 0 || Zone is not IFieldController) yield break;

            yield return _zoneManager.MoveCardFromFieldToGraveyard(this, Owner);
        }

        public IEnumerator Combat()
        {
            yield return _fieldManager.CombatMovement(this, new Point(
            Owner == Player.Player1 ? StatBlock.Stats.SPD :
            Owner == Player.Player2 ? -StatBlock.Stats.SPD : 0, 0));
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
        public IEnumerator OnEventRaised(ManaModifiedEvent eventData)
        {
            _lastManas = eventData.Mana.ManaTypes;
            UpdatePlayabilityAndCostText();
            yield break;
        }

        public bool ShouldReceiveEvent(ManaModifiedEvent eventData)
        {
            return eventData.Mana.Player == Owner && eventData.Mana.ManaTypes != null;
        }

        public IEnumerator OnEventRaised(CardChangedZonesEvent eventData)
        {
            if (_lastManas != null)
            {
                UpdatePlayabilityAndCostText();
            }
            yield break;
        }

        public bool ShouldReceiveEvent(CardChangedZonesEvent eventData)
        {
            return eventData.Card == this;
        }

        public IEnumerator OnEventRaised(GameStateEnteredEvent eventData)
        {
            if (_lastManas != null)
            {
                UpdatePlayabilityAndCostText();
            }
            yield break;
        }

        public bool ShouldReceiveEvent(GameStateEnteredEvent eventData)
        {
            return true;
        }

        public IEnumerator OnEventRaised(ActivePlayerChangedEvent eventData)
        {
            if (_lastManas != null)
            {
                UpdatePlayabilityAndCostText();
            }
            yield break;
        }

        public bool ShouldReceiveEvent(ActivePlayerChangedEvent eventData)
        {
            return true;
        }
        #endregion
    }
}