using Abraxas.Events;
using Abraxas.Events.Managers;
using Abraxas.Players.Managers;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Abraxas.Assets.AbraxasAssets.Scripts.VFX.Controllers
{
    public class TextureScrollGameController : MonoBehaviour, IGameEventListener<Event_ActivePlayerChanged>, IGameEventListener<Event_LocalPlayerChanged>
    {
        private TextureScroller _textureScroller;
        IEventManager _eventManager;
        IPlayerManager _playerManager;

        private Vector2 scrollLeft = new(-0.05f, 0.0f);
        private Vector2 scrollRight = new(0.05f, 0.0f);

        [Inject]
        public void Construct(IEventManager eventManager, IPlayerManager playerManager)
        {
            _eventManager = eventManager;
            _playerManager = playerManager;
            eventManager.AddListener(this as IGameEventListener<Event_ActivePlayerChanged>);
            eventManager.AddListener(this as IGameEventListener<Event_LocalPlayerChanged>);
        }

        void Awake()
        {
            _textureScroller = GetComponent<TextureScroller>();
        }

        void OnDestroy()
        {
            _eventManager.RemoveListener(this as IGameEventListener<Event_ActivePlayerChanged>);
            _eventManager.RemoveListener(this as IGameEventListener<Event_LocalPlayerChanged>);
        }

        public IEnumerator OnEventRaised(Event_ActivePlayerChanged eventData)
        {
            bool isPlayer1 = (eventData.Player == Players.Players.Player1) ? (_playerManager.LocalPlayer == Players.Players.Player1) : (_playerManager.LocalPlayer == Players.Players.Player2);
            _textureScroller.SetScrollDirection(isPlayer1 ? scrollLeft : scrollRight);
            yield break;
        }

        public bool ShouldReceiveEvent(Event_ActivePlayerChanged eventData)
        {
            return true;
        }

        public IEnumerator OnEventRaised(Event_LocalPlayerChanged eventData)
        {
            bool isPlayer1 = (eventData.Player == Players.Players.Player1) ? (_playerManager.LocalPlayer == Players.Players.Player1) : (_playerManager.LocalPlayer == Players.Players.Player2);
            _textureScroller.SetScrollDirection(isPlayer1 ? scrollLeft : scrollRight);
            yield break;
        }

        public bool ShouldReceiveEvent(Event_LocalPlayerChanged eventData)
        {
            return true;
        }
    }

}
