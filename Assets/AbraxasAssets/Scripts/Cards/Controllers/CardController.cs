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
        ICardModelReader _modelReader;
        ICardModelWriter _modelWriter;
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

        public void Initialize(ICardModelReader modelReader, ICardModelWriter modelWriter, ICardView view)
        {
            _modelReader = modelReader;
            _modelWriter = modelWriter;
            _view = view;

            _eventManager.AddListener(typeof(ManaModifiedEvent), this);
        }

        public class Factory : PlaceholderFactory<CardData, Player, ICardController>
        {

        }
        #endregion

        #region Properties
        public ICardModelReader Model => _modelReader;
        public ICardView View => _view;
        public IStatBlockModel StatBlock => _modelReader.StatBlock;
        public string Title { get => _modelReader.Title; set => _modelWriter.Title = value; }
        public Player Owner { get => _modelReader.Owner; set => _modelWriter.Owner = value; }
        public Player OriginalOwner { get => _modelReader.OriginalOwner; set => _modelWriter.OriginalOwner = value; }
        public Dictionary<StoneType, int> TotalCosts { get => _modelReader.TotalCosts;}
        public Point FieldPosition { get => _modelReader.FieldPosition; set => _modelWriter.FieldPosition = value; }
        public ICellController Cell { get => _modelReader.Cell; set => _modelWriter.Cell = value; }
        public IZoneController Zone { get => _modelReader.Zone; set => _modelWriter.Zone = value; }
        public bool Hidden { get => _modelReader.Hidden; set => _modelWriter.Hidden = value; }

        #endregion

        #region Methods
        public void OnDestroy()
        {
            _eventManager.RemoveListener(typeof(ManaModifiedEvent), this);
        }

        public IEnumerator PassHomeRow()
        {
            _healthManager.ModifyPlayerHealth(Model.Owner ==
                Player.Player1 ? Player.Player2 : Player.Player1,
                -Model.StatBlock[StatValues.ATK]);
            yield return _zoneManager.MoveCardFromFieldToDeck(this);
            _deckManager.RequestShuffleDeck(Model.Owner);
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
                Model.Owner == Player.Player1 ? Model.StatBlock[StatValues.MV] :
                Model.Owner == Player.Player2 ? -Model.StatBlock[StatValues.MV] : 0, 0));
        }
        public IEnumerator OnEventRaised(ManaModifiedEvent eventData)
        {
            if (eventData.Mana.Player == Owner && eventData.Mana.ManaTypes != null)
            {
                View.UpdateCostTextWithCastability(eventData);
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