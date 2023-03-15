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



    internal void CardMove(Card card)
    {
        OnCardMoved(card);
    }

    internal void CardDestroyed(Card card)
    {
        OnCardDestroyed(card);
    }

    internal void CardEnteredField(Card card)
    {
        OnCardEnteredField(card);
    }
}
