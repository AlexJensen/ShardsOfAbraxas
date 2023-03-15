using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class NextButton : MonoBehaviour, OnGameStateChange
{
    public const string BEGINNING_BUTTON_TEXT = "Start of Turn";
    public const string BEFORE_COMBAT_BUTTON_TEXT = "To Combat";
    public const string COMBAT_BUTTON_TEXT = "Combat";
    public const string AFTER_COMBAT_BUTTON_TEXT = "End Turn";
    public const string END_OF_TURN_TEXT = "Turn Ending";

    Button button;
    TMP_Text buttonStr;

    void Start()
    {
        Game.Instance.GameStateChanged += GameStateChanged;
        button = GetComponent<Button>();
        buttonStr = button.GetComponentInChildren<TMP_Text>();
    }

    public void GameStateChanged(State state)
    {
        if (state is BeginningState)
        {
            button.interactable = false;
            buttonStr.text = BEGINNING_BUTTON_TEXT;
        }
        else if (state is BeforeCombatState)
        {
            button.interactable = true;
            buttonStr.text = BEFORE_COMBAT_BUTTON_TEXT;
        }
        else if (state is CombatState)
        {
            button.interactable = false;
            buttonStr.text = COMBAT_BUTTON_TEXT;
        }
        else if (state is AfterCombatState)
        {
            button.interactable = true;
            buttonStr.text = AFTER_COMBAT_BUTTON_TEXT;
        }
        else if (state is EndState)
        {
            button.interactable = false;
            buttonStr.text = END_OF_TURN_TEXT;
        }
    }
}
