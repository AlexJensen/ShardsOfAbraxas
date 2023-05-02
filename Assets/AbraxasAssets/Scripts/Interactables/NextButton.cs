using Abraxas.Events;
using Abraxas.GameStates;
using Abraxas.Players;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Abraxas.UI
{
    [RequireComponent(typeof(Button))]
    public class NextButton : MonoBehaviour, IGameEventListener<GameStateEnteredEvent>
    {
        #region Settings
        Settings _buttonSettings;
        [Serializable]
        public class Settings
        {
            public string BeginningText;
            public string BeforeCombatText;
            public string CombatText;
            public string AfterCombatText;
            public string EndOfTurnText;
        }
        #endregion

        #region Dependencies
        IGameStateManager _gameStateManager;
        IEventManager _eventManager;
        IPlayerManager _playerManager;
        [Inject]
        public void Construct(Settings buttonSettings, IGameStateManager gameStateManager, IPlayerManager playerManager, IEventManager eventManager)
        {
            _buttonSettings = buttonSettings;
            _gameStateManager = gameStateManager;
            _eventManager = eventManager;
            _playerManager = playerManager;

            _eventManager.AddListener(typeof(GameStateEnteredEvent), this);
        }
        #endregion

        #region Fields
        Button _button;
        TMP_Text _buttonStr;
        #endregion

        #region Methods
        void Start()
        {
            _button = GetComponent<Button>();
            _buttonStr = _button.GetComponentInChildren<TMP_Text>();
        }

        private void OnDestroy()
        {
            _eventManager.RemoveListener(typeof(GameStateEnteredEvent), this);
        }

        public void NextButtonPressed()
        {
            _button.interactable = false;
            StartCoroutine( _gameStateManager.RequestNextGameState());
        }

        public IEnumerator OnEventRaised(GameStateEnteredEvent eventData)
        {
            GameState state = eventData.State;
            if (state is BeginningState)
            {
                _button.interactable = false;
                _buttonStr.text = _buttonSettings.BeginningText;
            }
            else if (state is BeforeCombatState)
            {
                _button.interactable = true;
                _buttonStr.text = _buttonSettings.BeforeCombatText;
            }
            else if (state is CombatState)
            {
                _button.interactable = false;
                _buttonStr.text = _buttonSettings.CombatText;
            }
            else if (state is AfterCombatState)
            {
                _button.interactable = true;
                _buttonStr.text = _buttonSettings.AfterCombatText;
            }
            else if (state is EndState)
            {
                _button.interactable = false;
                _buttonStr.text = _buttonSettings.EndOfTurnText;
            }

            if (_playerManager.LocalPlayer != _playerManager.ActivePlayer)
            {
                _button.interactable = false;
            }
            yield break;
        }
        #endregion
    }
}