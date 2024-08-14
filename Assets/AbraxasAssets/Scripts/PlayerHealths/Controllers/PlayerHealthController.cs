using Abraxas.Events;
using Abraxas.Events.Managers;
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

        readonly IEventManager _eventManager;
        [Inject]
        public PlayerHealthController(IEventManager eventManager)
        {
            _eventManager = eventManager;

            _eventManager.AddListener(typeof(Event_LocalPlayerChanged), this);
        }
        internal void Initialize(IPlayerHealthView view, IPlayerHealthModel model)
        {
            _model = model;
            _view = view;
        }

        public IEnumerator OnEventRaised(Event_LocalPlayerChanged eventData)
        {
            _model.Player = _view.Player;
            yield break;
        }

        public bool ShouldReceiveEvent(Event_LocalPlayerChanged eventData)
        {
            return true;
        }

        public class Factory : PlaceholderFactory<IPlayerHealthView, IPlayerHealthController>
        {

        }
        #endregion

        #region Properties
        public int HP
        {
            get => _model.HP;
            set => _model.HP = value;
        }
        public int MaxHP
        {
            get => _model.MaxHP;
            set => _model.MaxHP = value;
        }
        public Player Player
        {
            get => _model.Player;
            set => _model.Player = value;
        }
        #endregion
    }
}
