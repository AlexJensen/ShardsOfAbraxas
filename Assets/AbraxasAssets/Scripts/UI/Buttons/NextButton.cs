using Abraxas.Events;
using Abraxas.Events.Managers;
using Abraxas.Games.Managers;
using Abraxas.GameStates;
using Abraxas.Players;
using Abraxas.Players.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace Abraxas.UI
{
    public class NextButton : MonoBehaviour, IGameEventListener<Event_GameStateEntered>
    {
        #region Settings
        [Serializable]
        public class Settings
        {
            [Serializable]
            public struct NextButtonState
            {
                public string Text;
                public GameStates.GameStates State;
            }

            [SerializeField]
            private List<NextButtonState> _nextButtonStates = new();

            public string GetText(GameStates.GameStates state)
            {
                foreach (var nextState in from nextState in _nextButtonStates
                                          where nextState.State == state
                                          select nextState)
                {
                    return nextState.Text;
                }

                return "NO STATE FOUND";
            }
        }
        #endregion

        #region Dependencies
        IGameManager _gameManager;
        IGameStateManager _gameStateManager;
        IEventManager _eventManager;
        IPlayerManager _playerManager;
        Settings _buttonSettings;
        Player.Settings _playerSettings;
        [Inject]
        public void Construct(Settings buttonSettings, Player.Settings playerSettings, IGameManager gameManager, IGameStateManager gameStateManager, IPlayerManager playerManager, IEventManager eventManager)
        {
            _buttonSettings = buttonSettings;
            _playerSettings = playerSettings;
            _gameManager = gameManager;
            _gameStateManager = gameStateManager;
            _eventManager = eventManager;
            _playerManager = playerManager;

            _eventManager.AddListener(this);
        }
        #endregion

        #region Fields
        Button _button;
        Image _image;
        TMP_Text _buttonStr;
        #endregion

        #region Methods
        void Start()
        {
            _button = GetComponent<Button>();
            _image = GetComponent<Image>();
            _buttonStr = _button.GetComponentInChildren<TMP_Text>();
        }

        private void OnDestroy()
        {
            _eventManager.RemoveListener(this);
        }

        public void NextButtonPressed()
        {
            _button.interactable = false;
            _gameStateManager.RequestNextGameState();
        }

        public IEnumerator OnEventRaised(Event_GameStateEntered eventData)
        {
            GameState state = eventData.State;
            _button.interactable = _gameManager.IsAnyPlayerInputAvailable();
            _buttonStr.text = _buttonSettings.GetText(state.CurrentState);
            _image.color = _playerSettings.GetPlayerDetails(_playerManager.LocalPlayer == Players.Players.Player1 ? _playerManager.ActivePlayer : _playerManager.ActivePlayer == Players.Players.Player1 ? Players.Players.Player2 : Players.Players.Player1).color;
            if (_playerManager.LocalPlayer != _playerManager.ActivePlayer)
            {
                _button.interactable = false;
            }
            yield break;
        }

        public bool ShouldReceiveEvent(Event_GameStateEntered eventData)
        {
            return true;
        }
        #endregion
    }
}