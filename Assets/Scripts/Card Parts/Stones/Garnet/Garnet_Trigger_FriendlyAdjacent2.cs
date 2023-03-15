using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garnet_Trigger_FriendlyAdjacent2 : TriggerStone
{
    List<Vector2Int> adjacents;
    protected void Awake()
    {
        cost = 1;
        stoneType = StoneData.StoneType.TOURMALINE;
        info = "Whenever a friendly packet moves into an adjacent square:";

        adjacents = new List<Vector2Int>
        {
            new Vector2Int(1,0),
            new Vector2Int(-1,0),
            new Vector2Int(0,1),
            new Vector2Int(0,-1)
        };
    }

    private void OnEnable()
    {
        Events.Instance.OnCardEnteredField += OnCardEnteredField;
    }

    private void OnDisable()
    {
        Events.Instance.OnCardEnteredField -= OnCardEnteredField;
    }

    private void OnCardEnteredField(params object[] vals)
    {
        if (!Utilities.ValidateParam<Card>(this, vals[0]))
            return;
        Card triggeringCard = (Card)vals[0];
        if (card.Equals(triggeringCard))
        {
            Events.Instance.OnCardEnteredField -= OnCardEnteredField;
            Events.Instance.OnCardMoved += OnCardMove;
        }
    }

    private void OnCardMove(params object[] vals)
    {
        if (!Utilities.ValidateParam<Card>(this, vals[0]))
            return;
        Card triggeringCard = (Card)vals[0];
        foreach (Vector2Int adjacent in adjacents)
        {
            if (card.fieldPos + adjacent == triggeringCard.fieldPos && card.controller == triggeringCard.controller)
            {
                InvokeTrigger(triggeringCard);
            }
        }
    }
}
