using Abraxas.Behaviours.Game;
using Abraxas.Scripts.States;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Abraxas.Behaviours.Interactables
{
    [RequireComponent(typeof(Button))]
    public class NextButton : MonoBehaviour, OnGameStateChange
    {
        #region Dependency Injections
        [Inject] readonly GameManager _gameManager;
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
            _gameManager.GameStateChanged += GameStateChanged;
            _button = GetComponent<Button>();
            _buttonStr = _button.GetComponentInChildren<TMP_Text>();
        }

        public void GameStateChanged(GameState state)
        {
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
        }
    }
}