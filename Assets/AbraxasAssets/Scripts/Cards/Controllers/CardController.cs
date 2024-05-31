using Abraxas.Cards.Data;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.Cells.Controllers;
using Abraxas.Core;
using Abraxas.Events;
using Abraxas.Events.Managers;
using Abraxas.Health.Managers;

using Abraxas.StatBlocks.Controllers;
using Abraxas.StatBlocks.Data;

using Abraxas.Stones;
using Abraxas.Stones.Controllers;
using Abraxas.Unity.Interfaces;
using Abraxas.Zones.Controllers;
using Abraxas.Zones.Decks.Managers;
using Abraxas.Zones.Fields.Managers;
using Abraxas.Zones.Managers;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Zenject;
using Player = Abraxas.Players.Players;

namespace Abraxas.Cards.Controllers
{
    /// <summary>
    /// CardController facilitates communication between the card gameobject's model, view, managers, and other controllers.
    /// </summary>
    class CardController : ICardController, IGameEventListener<ManaModifiedEvent>,
                                            IGameEventListener<CardChangedZonesEvent>

    {
        #region Dependencies
        ICardModel _model;
        ICardView _view;

        readonly IZoneManager _zoneManager;
        readonly IEventManager _eventManager;
        readonly IPlayerHealthManager _healthManager;
        readonly IFieldManager _fieldManager;
        readonly IDeckManager _deckManager;

        public CardController(IZoneManager zoneManager, IDeckManager deckManager, IEventManager eventManager,
                              IPlayerHealthManager healthManager, IFieldManager fieldManager)
        {
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
        }


        public void OnDestroy()
        {
            _eventManager.RemoveListener(typeof(ManaModifiedEvent), this as IGameEventListener<ManaModifiedEvent>);

            _eventManager.RemoveListener(typeof(CardChangedZonesEvent), this as IGameEventListener<CardChangedZonesEvent>);
        }


        public class Factory : PlaceholderFactory<CardData, ICardController>
        {

        }
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
            get => _model.Zone; set
            {
                PreviousZone = Zone;
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
            _healthManager.ModifyPlayerHealth(_model.Owner ==
                Player.Player1 ? Player.Player2 : Player.Player1,

                -_model.StatBlock.Stats.ATK);

            yield return _zoneManager.MoveCardFromFieldToDeck(this, Owner);
            _deckManager.ShuffleDeck(_model.Owner);
        }
        public IEnumerator Fight(ICardController opponent)
        {
            if (opponent.Owner == Owner) yield break;

            IStatBlockController collided = opponent.StatBlock;

            StatData collidedStats = collided.Stats;
            StatData stats = _model.StatBlock.Stats;

            collidedStats.DEF -= _model.StatBlock.Stats.ATK;
            stats.DEF -= collidedStats.ATK;

            collided.Stats = collidedStats;
            _model.StatBlock.Stats = stats;


            yield return Utilities.WaitForCoroutines(
                opponent.CheckDeath(),
                CheckDeath());
        }
        public IEnumerator CheckDeath()
        {

            if (_model.StatBlock.Stats.DEF > 0) yield break;

            yield return _zoneManager.MoveCardFromFieldToGraveyard(this, Owner);
        }
        public IEnumerator Combat()
        {
            yield return _fieldManager.CombatMovement(this, new Point(

                _model.Owner == Player.Player1 ? _model.StatBlock.Stats.SPD :
                _model.Owner == Player.Player2 ? -_model.StatBlock.Stats.SPD : 0, 0));

        }

        public void ChangeScale(PointF pointF, float scaleCardToOverlayTime)
        {
            _view.ChangeScale(pointF, scaleCardToOverlayTime);
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
        #endregion

        #region Delegate Methods
        public IEnumerator OnEventRaised(ManaModifiedEvent eventData)
        {
            _lastManas = eventData.Mana.ManaTypes;
            _view.UpdateCostTextWithCastability(eventData.Mana.ManaTypes);
            yield break;
        }

        List<Manas.ManaType> _lastManas;
        public IEnumerator OnEventRaised(CardChangedZonesEvent eventData)
        {
            if (_lastManas != null)
            {

                _view.UpdateCostTextWithCastability(_lastManas);

            }
            yield break;
        }

        public bool ShouldReceiveEvent(ManaModifiedEvent eventData)
        {
            return eventData.Mana.Player == Owner && eventData.Mana.ManaTypes != null;
        }

        public bool ShouldReceiveEvent(CardChangedZonesEvent eventData)
        {
            return eventData.Card == this;
        }
        #endregion
    }
}