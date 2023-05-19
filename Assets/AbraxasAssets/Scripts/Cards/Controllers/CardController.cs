using Abraxas.Cards.Data;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.Cells.Controllers;
using Abraxas.Core;
using Abraxas.Events;
using Abraxas.StatBlocks;
using Abraxas.StatBlocks.Models;
using Abraxas.Stones;
using Abraxas.Zones.Controllers;
using Abraxas.Zones.Decks.Managers;
using Abraxas.Zones.Fields.Managers;
using Abraxas.Zones.Managers;
using Abraxas.Zones.Overlays.Managers;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Zenject;
using Player = Abraxas.Players.Players;

namespace Abraxas.Cards.Controllers
{
    class CardController : ICardController, IGameEventListener<ManaModifiedEvent>
    {
        #region Dependencies
        ICardModel _model;
        ICardView _view;
        readonly IZoneManager _zoneManager;
        readonly IEventManager _eventManager;
        readonly IHealthManager _healthManager;
        readonly IFieldManager _fieldManager;
        readonly IDeckManager _deckManager;
        readonly IOverlayManager _overlayManager;
        public CardController(IZoneManager zoneManager, IDeckManager deckManager, IEventManager eventManager, IHealthManager healthManager,
                              IFieldManager fieldManager, IOverlayManager overlayManager)
        {
            _zoneManager = zoneManager;
            _eventManager = eventManager;
            _healthManager = healthManager;
            _fieldManager = fieldManager;
            _deckManager = deckManager;
            _overlayManager = overlayManager;
        }

        public void Initialize(ICardModel model, ICardView view)
        {
            _model = model;
            _view = view;

            _eventManager.AddListener(typeof(ManaModifiedEvent), this);
        }

        public class Factory : PlaceholderFactory<CardData, Player, ICardController>
        {

        }
        #endregion

        #region Properties
        public ICardModel Model => _model;
        public ICardView View => _view;
        public IStatBlockModel StatBlock => _model.StatBlock;
        public string Title { get => ((ICardModelReader)_model).Title; set => ((ICardModelWriter)_model).Title = value; }
        public Player Owner { get => ((ICardModelReader)_model).Owner; set => ((ICardModelWriter)_model).Owner = value; }
        public Player OriginalOwner { get => ((ICardModelReader)_model).OriginalOwner; set => ((ICardModelWriter)_model).OriginalOwner = value; }
        public Dictionary<StoneType, int> TotalCosts { get => _model.TotalCosts;}
        public Point FieldPosition { get => ((ICardModelReader)_model).FieldPosition; set => ((ICardModelWriter)_model).FieldPosition = value; }
        public ICellController Cell { get => ((ICardModelReader)_model).Cell; set => ((ICardModelWriter)_model).Cell = value; }
        public IZoneController Zone { get => ((ICardModelReader)_model).Zone; set => ((ICardModelWriter)_model).Zone = value; }
        public bool Hidden { get => ((ICardModelReader)_model).Hidden; set => ((ICardModelWriter)_model).Hidden = value; }

        #endregion

        #region Methods
        public void OnDestroy()
        {
            _eventManager.RemoveListener(typeof(ManaModifiedEvent), this);
        }

        public IEnumerator PassHomeRow()
        {
            _healthManager.ModifyPlayerHealth(((ICardModelReader)_model).Owner ==
                Player.Player1 ? Player.Player2 : Player.Player1,
                -Model.StatBlock[StatValues.ATK]);
            yield return _zoneManager.MoveCardFromFieldToDeck(this);
            _deckManager.RequestShuffleDeck(((ICardModelReader)_model).Owner);
        }

        public IEnumerator Fight(ICardController opponent)
        {
            IStatBlockModel collidedStats = opponent.StatBlock;
            collidedStats[StatValues.DEF] -= Model.StatBlock[StatValues.ATK];
            Model.StatBlock[StatValues.DEF] -= collidedStats[StatValues.ATK];

            yield return Utilities.WaitForCoroutines(
                opponent.CheckDeath(),
                CheckDeath());
        }

        public IEnumerator CheckDeath()
        {
            if (Model.StatBlock[StatValues.DEF] <= 0)
            {
                yield return _zoneManager.MoveCardFromFieldToGraveyard(this);
            }
        }
        public IEnumerator Combat()
        {
            yield return _fieldManager.MoveCardAndFight(this, new Point(
                ((ICardModelReader)_model).Owner == Player.Player1 ? Model.StatBlock[StatValues.MV] :
                ((ICardModelReader)_model).Owner == Player.Player2 ? -Model.StatBlock[StatValues.MV] : 0, 0));
        }
        public IEnumerator OnEventRaised(ManaModifiedEvent eventData)
        {
            if (eventData.Mana.Player == Owner && eventData.Mana.ManaTypes != null)
            {
                _view.UpdateCostTextWithCastability(eventData);
            }
            yield break;
        }
        public void AddToOverlay()
        {
            _overlayManager.SetCard(_view);
        }
        public void ScaleToRectangle(PointF dimensions, float time)
        {
            _view.ChangeScale(dimensions, time);
        }
        #endregion
    }
}