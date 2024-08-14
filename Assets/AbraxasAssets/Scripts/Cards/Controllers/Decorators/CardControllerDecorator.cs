using Abraxas.Cards.Models;
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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Zenject;
using Player = Abraxas.Players.Players;

namespace Abraxas.Cards.Controllers
{
    class CardControllerDecorator : ICardControllerInternal,
                                    IGameEventListener<Event_ManaModified>,
                                    IGameEventListener<Event_CardChangedZones>,
                                    IGameEventListener<Event_GameStateEntered>,
                                    IGameEventListener<Event_ActivePlayerChanged>
                                    
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
        public void Initialize(
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

            InitializeListeners();
        }

        public void OnDestroy()
        {
            RemoveListeners();
        }

        public virtual void InitializeListeners()
        {
            _eventManager.AddListener(typeof(Event_ManaModified), this as IGameEventListener<Event_ManaModified>);
            _eventManager.AddListener(typeof(Event_CardChangedZones), this as IGameEventListener<Event_CardChangedZones>);
            _eventManager.AddListener(typeof(Event_GameStateEntered), this as IGameEventListener<Event_GameStateEntered>);
            _eventManager.AddListener(typeof(Event_ActivePlayerChanged), this as IGameEventListener<Event_ActivePlayerChanged>);
        }

        public virtual void RemoveListeners()
        {
            _eventManager.RemoveListener(typeof(Event_ManaModified), this as IGameEventListener<Event_ManaModified>);
            _eventManager.RemoveListener(typeof(Event_CardChangedZones), this as IGameEventListener<Event_CardChangedZones>);
            _eventManager.RemoveListener(typeof(Event_GameStateEntered), this as IGameEventListener<Event_GameStateEntered>);
            _eventManager.RemoveListener(typeof(Event_ActivePlayerChanged), this as IGameEventListener<Event_ActivePlayerChanged>);
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
        public virtual IEnumerator PassHomeRow()
        {
            _healthManager.ModifyPlayerHealth(Owner ==
                Player.Player1 ? Player.Player2 : Player.Player1, -StatBlock.Stats.ATK);

            yield return _zoneManager.MoveCardFromFieldToDeck(GetBaseCard(), Owner, 0, true);
        }
        public virtual IEnumerator Fight(ICardController opponent)
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

        public virtual IEnumerator RangedAttack(ICardController opponent)
        {
            if (opponent.Owner == Owner) yield break;

            IStatBlockController collided = opponent.StatBlock;

            StatData collidedStats = collided.Stats;

            collidedStats.DEF -= StatBlock.Stats.ATK;

            collided.Stats = collidedStats;

            yield return Utilities.WaitForCoroutines(
                               opponent.CheckDeath());
        }

        public virtual IEnumerator CheckDeath()
        {

            if (StatBlock.Stats.DEF > 0 || Zone is not IFieldController) yield break;

            yield return _zoneManager.MoveCardFromFieldToGraveyard(GetBaseCard(), Owner);
        }

        public virtual IEnumerator Combat()
        {
            yield return _fieldManager.CombatMovement(GetBaseCard(), new Point(Owner == Player.Player1? StatBlock.Stats.SPD:-StatBlock.Stats.SPD, 0));
        }

        public virtual void ChangeScale(PointF pointF, float scaleCardToOverlayTime) => _view.ChangeScale(pointF, scaleCardToOverlayTime);
        public virtual void SetToInitialScale() => _view.SetToInitialScale();
        public virtual void SetCardPositionToMousePosition() => _view.SetCardPositionToMousePosition();
        public virtual string GetCostText() => _view.GetCostText();
        public virtual IEnumerator MoveToCell(ICellController cell, float moveCardTime) => _view.MoveToCell(cell, moveCardTime);
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

        public virtual void UpdatePlayabilityAndCostText()
        {
            bool isPlayable = DeterminePlayability();
            _view.UpdateCostTextWithManaTypes(LastManas, TotalCosts, isPlayable);
        }
        #endregion

        #region Delegate Methods
        public virtual IEnumerator OnEventRaised(Event_ManaModified eventData)
        {
            LastManas = eventData.Data.ManaTypes;
            UpdatePlayabilityAndCostText();
            yield break;
        }

        public virtual bool ShouldReceiveEvent(Event_ManaModified eventData) => eventData.Data.Player == Owner && eventData.Data.ManaTypes != null;

        public virtual IEnumerator OnEventRaised(Event_CardChangedZones eventData)
        {
            if (Zone is IFieldController)
            {
                // Apply summoning sickness when a card enters the field
                ApplyStatusEffect(new StatusEffect_SummoningSickness());
            }
            if (LastManas != null)
            {
                UpdatePlayabilityAndCostText();
            }
            yield break;
        }

        public virtual bool ShouldReceiveEvent(Event_CardChangedZones eventData) => eventData.Data.Equals(GetBaseCard());

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
