using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Events : Singleton<Events>
{
    internal delegate void GameEvent(params object[] vals);

    internal event GameEvent OnCardMoved;
    internal event GameEvent OnCardDestroyed;
    internal event GameEvent OnCardEnteredField;
    internal event GameEvent OnBeginningStateStarted;
    internal event GameEvent OnBeforeCombatStarted;

    internal void CardMove(Card card)
    {
        if (OnCardMoved != null) OnCardMoved(card);
    }
    internal void CardDestroyed(Card card)
    {
        if (OnCardDestroyed != null) OnCardDestroyed(card);
    }
    internal void CardEnteredField(Card card)
    {
        if (OnCardEnteredField != null) OnCardEnteredField(card);
    }
    internal void BeginningStateStarted()
    {
        if (OnBeginningStateStarted != null) OnBeginningStateStarted();
    }
    internal void BeforeCombatStarted()
    {
        if (OnBeforeCombatStarted != null) OnBeforeCombatStarted();
    }
}
