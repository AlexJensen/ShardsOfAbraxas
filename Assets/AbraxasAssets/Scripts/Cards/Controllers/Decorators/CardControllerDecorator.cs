using Abraxas.Cards.Controllers;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.Cells.Controllers;
using Abraxas.Core;
using Abraxas.Events.Managers;
using Abraxas.GameStates;
using Abraxas.GameStates.Managers;
using Abraxas.Health.Managers;
using Abraxas.Manas;
using Abraxas.Players.Managers;
using Abraxas.StatBlocks.Controllers;
using Abraxas.StatBlocks.Data;
using Abraxas.StatusEffects;
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
    class CardControllerDecorator : ICardController
    {
        protected ICardController _innerController;
        protected ICardModel _model;
        protected ICardView _view;

        protected  IPlayerManager _playerManager;
        protected  IGameStateManager _gameStateManager;
        protected  IZoneManager _zoneManager;
        protected  IEventManager _eventManager;
        protected  IPlayerHealthManager _healthManager;
        protected  IFieldManager _fieldManager;

        public CardControllerDecorator(ICardController innerController, ICardModel model, ICardView view)
        {
            _innerController = innerController;
            _model = model;
            _view = view;
        }

        // Use method injection to inject the manager dependencies
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
        }

        public virtual IStatBlockController StatBlock => _model.StatBlock;
        public virtual List<IStoneController> Stones => _model.Stones;
        public virtual string Title { get => _model.Title; set => _model.Title = value; }
        public virtual Player Owner { get => _model.Owner; set => _model.Owner = value; }
        public virtual Player OriginalOwner { get => _model.OriginalOwner; set => _model.OriginalOwner = value; }
        public virtual Dictionary<StoneType, int> TotalCosts => _model.TotalCosts;
        public virtual ICellController Cell { get => _model.Cell; set => _model.Cell = value; }
        public virtual IZoneController Zone { get => _model.Zone; set => _model.Zone = value; }
        public virtual IZoneController PreviousZone { get => _innerController.PreviousZone; set => _innerController.PreviousZone = value; }
        public virtual bool Hidden { get => _model.Hidden; set => _model.Hidden = value; }
        public virtual ITransformManipulator TransformManipulator => (ITransformManipulator)_view;
        public virtual IImageManipulator ImageManipulator => (IImageManipulator)_view;
        public virtual RectTransformMover RectTransformMover => _view.RectTransformMover;
        public List<ManaType> LastManas { get => _innerController.LastManas; set => _innerController.LastManas = value; }
        public virtual void ApplyStatusEffect(IStatusEffect effect) => _innerController.ApplyStatusEffect(effect);
        public virtual bool HasStatusEffect<T>() where T : IStatusEffect => _innerController.HasStatusEffect<T>();
        public virtual void RemoveStatusEffect<T>() where T : IStatusEffect => _innerController.RemoveStatusEffect<T>();
        public virtual IEnumerator PassHomeRow()
        {
            _healthManager.ModifyPlayerHealth(Owner ==
                Player.Player1 ? Player.Player2 : Player.Player1,

                -StatBlock.Stats.ATK);

            yield return _zoneManager.MoveCardFromFieldToDeck(this, Owner, 0, true);
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

            yield return _zoneManager.MoveCardFromFieldToGraveyard(this, Owner);
        }

        public virtual IEnumerator Combat()
        {
                yield return _fieldManager.CombatMovement(this, new Point(
                    Owner == Player.Player1 ? StatBlock.Stats.SPD :
                    Owner == Player.Player2 ? -StatBlock.Stats.SPD : 0, 0));
        }
        public virtual void ChangeScale(PointF pointF, float scaleCardToOverlayTime) => _view.ChangeScale(pointF, scaleCardToOverlayTime);
        public virtual void SetToInitialScale() => _view.SetToInitialScale();
        public virtual void SetCardPositionToMousePosition() => _view.SetCardPositionToMousePosition();
        public virtual string GetCostText() => _view.GetCostText();
        public virtual IEnumerator MoveToCell(ICellController cell, float moveCardTime) => _view.MoveToCell(cell, moveCardTime);
        public bool DeterminePlayability()
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

        public void UpdatePlayabilityAndCostText()
        {
            bool isPlayable = DeterminePlayability();
            _view.UpdateCostTextWithManaTypes(LastManas, TotalCosts, isPlayable);
        }
    }
}
