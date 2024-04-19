using Abraxas.Events;
using Abraxas.GameStates;
using Abraxas.Players.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Linq;
using Abraxas.Players;
using Abraxas.Events.Managers;

namespace Abraxas.UI
{
    public class NextButton : MonoBehaviour, IGameEventListener<GameStateEnteredEvent>
    {
        #region Settings
        [Serializable]
        public class Settings
        {
            [Serializable]
            public struct NextButtonState
            {
                public string Text;
                public bool Interactable;
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

            public bool GetInteractable(GameStates.GameStates state)
            {
                foreach (var nextState in from nextState in _nextButtonStates
                                          where nextState.State == state
                                          select nextState)
                {
                    return nextState.Interactable;
                }

                return false;
            }
        }
        #endregion

        #region Dependencies
        IGameStateManager _gameStateManager;
        IEventManager _eventManager;
        IPlayerManager _playerManager;
        Settings _buttonSettings;
        Player.Settings _playerSettings;
        [Inject]
        public void Construct(Settings buttonSettings, Player.Settings playerSettings, IGameStateManager gameStateManager, IPlayerManager playerManager, IEventManager eventManager)
        {
            _buttonSettings = buttonSettings;
            _playerSettings = playerSettings;
            _gameStateManager = gameStateManager;
            _eventManager = eventManager;
            _playerManager = playerManager;

            _eventManager.AddListener(typeof(GameStateEnteredEvent), this);
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
            _eventManager.RemoveListener(typeof(GameStateEnteredEvent), this);
        }

        public void NextButtonPressed()
        {
            _button.interactable = false;
            _gameStateManager.RequestNextGameState();
        }

        public IEnumerator OnEventRaised(GameStateEnteredEvent eventData)
        {
            GameState state = eventData.State;
            _button.interactable = _buttonSettings.GetInteractable(state.CurrentState);
            _buttonStr.text = _buttonSettings.GetText(state.CurrentState);
            _image.color = _playerSettings.GetPlayerDetails(_playerManager.ActivePlayer).color;
            if (_playerManager.LocalPlayer != _playerManager.ActivePlayer)
            {
                _button.interactable = false;
            }
            yield break;
        }

        public bool ShouldReceiveEvent(GameStateEnteredEvent eventData)
        {
            return true;
        }
        #endregion
    }
}