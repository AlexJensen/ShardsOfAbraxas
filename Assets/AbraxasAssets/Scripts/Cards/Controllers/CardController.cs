using Abraxas.Cards.Models;
using Abraxas.Cards.Views;

using Abraxas.Core;
using Abraxas.Game;
using Abraxas.Events;
using Abraxas.Stones;
using Abraxas.Zones.Fields;

using System.Collections;
using System.Collections.Generic;
using System.Drawing;

using Player = Abraxas.Players.Players;
using Zone = Abraxas.Zones.Zones;


namespace Abraxas.Cards.Controllers
{
    class CardController : ICardController, IGameEventListener<ManaModifiedEvent>
    {
        #region Dependencies
        readonly IGameManager _gameManager;
        readonly IEventManager _eventManager;
        readonly IHealthManager _healthManager;
        readonly IFieldManager _fieldManager;

        readonly ICardModelReader _modelReader;
        readonly ICardModelWriter _modelWriter;
        readonly ICardView _view;
        readonly ICardDragHandler _dragHandler;
        readonly ICardMouseOverHandler _mouseOverHandler;

        public CardController(ICardModelReader modelReader, ICardModelWriter modelWriter, ICardView view,
                              ICardDragHandler dragHandler, ICardMouseOverHandler mouseOverHandler,
                              IGameManager gameManager, IEventManager eventManager, IHealthManager healthManager,
                              IFieldManager fieldManager)
        {
            _modelReader = modelReader;
            _modelWriter = modelWriter;
            _view = view;
            _dragHandler = dragHandler;
            _mouseOverHandler = mouseOverHandler;

            _gameManager = gameManager;
            _eventManager = eventManager;
            _healthManager = healthManager;
            _fieldManager = fieldManager;
            
            _eventManager.AddListener(typeof(ManaModifiedEvent), this);
        }
        #endregion

        #region Properties
        public ICardModelReader Model => _modelReader;
        public ICardView View => _view;
        public ICardDragHandler DragHandler => _dragHandler;
        public ICardMouseOverHandler MouseOverHandler => _mouseOverHandler;

        public string Title { get => _modelReader.Title; set => _modelWriter.Title = value; }
        public Player Owner { get => _modelReader.Owner; set => _modelWriter.Owner = value; }
        public Player OriginalOwner { get => _modelReader.OriginalOwner; set => _modelWriter.OriginalOwner = value; }
        public List<IStoneController> Stones { get => _modelReader.Stones; set => _modelWriter.Stones = value; }
        public StatBlock StatBlock { get => _modelReader.StatBlock; set => _modelWriter.StatBlock = value; }
        public Dictionary<StoneType, int> TotalCosts { get => _modelReader.TotalCosts;}
        public Point FieldPosition { get => _modelReader.FieldPosition; set => _modelWriter.FieldPosition = value; }
        public ICellController Cell { get => _modelReader.Cell; set => _modelWriter.Cell = value; }
        public Zone Zone { get => _modelReader.Zone; set => _modelWriter.Zone = value; }
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
                -Model.StatBlock[StatBlock.StatValues.ATK]);
            yield return _gameManager.MoveCardFromFieldToDeck(this);
        }

        public IEnumerator Fight(ICardController opponent)
        {
            StatBlock collidedStats = opponent.Model.StatBlock;
            collidedStats[StatBlock.StatValues.DEF] -= Model.StatBlock[StatBlock.StatValues.ATK];
            Model.StatBlock[StatBlock.StatValues.DEF] -= collidedStats[StatBlock.StatValues.ATK];

            yield return Utilities.WaitForCoroutines(
                opponent.CheckDeath(),
                CheckDeath());
        }

        public IEnumerator CheckDeath()
        {
            if (Model.StatBlock[StatBlock.StatValues.DEF] <= 0)
            {
                yield return _gameManager.MoveCardFromFieldToGraveyard(this);
            }
        }

        public IEnumerator Combat()
        {
            yield return _fieldManager.MoveCardAndFight(this, new Point(
                Model.Owner == Player.Player1 ? Model.StatBlock[StatBlock.StatValues.MV] :
                Model.Owner == Player.Player2 ? -Model.StatBlock[StatBlock.StatValues.MV] : 0, 0));
        }

        public IEnumerator OnEventRaised(ManaModifiedEvent eventData)
        {
            if (eventData.Mana.Player == Owner && eventData.Mana.ManaTypes != null)
            {
                View.UpdateCostTextWithCastability(eventData);
            }
            yield break;
        }
        #endregion
    }
}