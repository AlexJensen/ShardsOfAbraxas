using Abraxas.Events;
using Abraxas.Events.Managers;
using Abraxas.Games.Managers;
using Abraxas.Health.Models;
using Abraxas.Health.Views;
using System.Collections;
using Zenject;
using Player = Abraxas.Players.Players;

namespace Abraxas.Health.Controllers
{
    /// <summary>
    /// PlayerHealthController facilitates communication between the player health model and view.
    /// </summary>
    class PlayerHealthController : IPlayerHealthController,
        IGameEventListener<Event_LocalPlayerChanged>
    {
        #region Dependencies
        IPlayerHealthModel _model;
        IPlayerHealthView _view;

        readonly IGameManager _gameManager;
        readonly IEventManager _eventManager;
        [Inject]
        public PlayerHealthController(IGameManager gameManager, IEventManager eventManager)
        {
            _gameManager = gameManager;
            _eventManager = eventManager;

            _eventManager.AddListener(this);
        }
        internal void Initialize(IPlayerHealthView view, IPlayerHealthModel model)
        {
            _model = model;
            _view = view;
        }

        public class Factory : PlaceholderFactory<IPlayerHealthView, IPlayerHealthController> { }
        #endregion

        #region Properties
        public Player Player
        {
            get => _model.Player;
            set => _model.Player = value;
        }

        public int HP => _model.HP;

        public int MaxHP => _model.MaxHP;
        #endregion

        #region Methods
        public IEnumerator OnEventRaised(Event_LocalPlayerChanged eventData)
        {
            _model.Player = _view.Player;
            yield break;
        }

        public bool ShouldReceiveEvent(Event_LocalPlayerChanged eventData)
        {
            return true;
        }

        public IEnumerator SetHealth(int health)
        {
            _model.HP = health;
            yield return _eventManager.RaiseEvent(new Event_PlayerHealthModified(this));
            if (HP <= 0)
            {
                yield return _gameManager.EndGame(Player);
            }
        }

        public IEnumerator SetMaxHealth(int health)
        {
            _model.MaxHP = health;
            if (HP > MaxHP)
            {
                yield return SetHealth(MaxHP);
            }
            yield return _eventManager.RaiseEvent(new Event_PlayerHealthModified(this));

        }
        #endregion
    }
}
