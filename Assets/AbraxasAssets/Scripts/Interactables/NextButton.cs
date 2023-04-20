using Abraxas.Events;
using Abraxas.Game;
using Abraxas.GameStates;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Abraxas.UI
{
    [RequireComponent(typeof(Button))]
    public class NextButton : MonoBehaviour, IGameEventListener<GameStateChangedEvent>
    {
        #region Dependencies
        IGameManager _gameManager;
        IEventManager _eventManager;
        [Inject]
        public void Construct(IGameManager gameManager, IEventManager eventManager)
        {
            _gameManager = gameManager;
            _eventManager = eventManager;

            _eventManager.AddListener(typeof(GameStateChangedEvent), this);
        }
        #endregion

        public const string BEGINNING_BUTTON_TEXT = "Start of Turn";
        public const string BEFORE_COMBAT_BUTTON_TEXT = "To Combat";
        public const string COMBAT_BUTTON_TEXT = "Combat";
        public const string AFTER_COMBAT_BUTTON_TEXT = "End Turn";
        public const string END_OF_TURN_TEXT = "Turn Ending";

        Button _button;
        TMP_Text _buttonStr;

        void Start()
        {
            _button = GetComponent<Button>();
            _buttonStr = _button.GetComponentInChildren<TMP_Text>();
        }

        private void OnDestroy()
        {
            _eventManager.RemoveListener(typeof(GameStateChangedEvent), this);
        }

        public void NextButtonPressed()
        {
            StartCoroutine(_gameManager.BeginNextGameState());
        }

        public IEnumerator OnEventRaised(GameStateChangedEvent eventData)
        {
            GameState state = eventData.State;
            if (state is BeginningState)
            {
                _button.interactable = false;
                _buttonStr.text = BEGINNING_BUTTON_TEXT;
            }
            else if (state is BeforeCombatState)
            {
                _button.interactable = true;
                _buttonStr.text = BEFORE_COMBAT_BUTTON_TEXT;
            }
            else if (state is CombatState)
            {
                _button.interactable = false;
                _buttonStr.text = COMBAT_BUTTON_TEXT;
            }
            else if (state is AfterCombatState)
            {
                _button.interactable = true;
                _buttonStr.text = AFTER_COMBAT_BUTTON_TEXT;
            }
            else if (state is EndState)
            {
                _button.interactable = false;
                _buttonStr.text = END_OF_TURN_TEXT;
            }
            yield break;
        }
    }
}