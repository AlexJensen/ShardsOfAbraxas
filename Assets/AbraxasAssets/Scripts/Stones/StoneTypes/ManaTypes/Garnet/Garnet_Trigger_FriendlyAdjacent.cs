using Abraxas.Behaviours.Cards;
using Abraxas.Behaviours.Data;
using Abraxas.Behaviours.Events;
using Abraxas.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Abraxas.Behaviours.Stones
{
    public class Garnet_Trigger_FriendlyAdjacent : TriggerStone
    {
        List<Vector2Int> _adjacents;

        public override int Cost { get; set; }
        public override string Info { get; set; }
        public override StoneData.StoneType StoneType { get; set; }

        protected void Awake()
        {
            Cost = 3;
            StoneType = StoneData.StoneType.GARNET;
            Info = "Whenever a friendly packet moves into an adjacent square:";

            _adjacents = new List<Vector2Int>
        {
            new Vector2Int(1,0),
            new Vector2Int(-1,0),
            new Vector2Int(0,1),
            new Vector2Int(0,-1)
        };
        }

        private void OnEnable()
        {
            EventManager.Instance.OnCardEnteredField += OnCardEnteredField;
        }

        private void OnDisable()
        {
            EventManager.Instance.OnCardEnteredField -= OnCardEnteredField;
        }

        private void OnCardEnteredField(params object[] vals)
        {
            if (!Utilities.ValidateParam<Card>(this, vals[0]))
                return;
            Card triggeringCard = (Card)vals[0];
            if (card.Equals(triggeringCard))
            {
                EventManager.Instance.OnCardEnteredField -= OnCardEnteredField;
                EventManager.Instance.OnCardMoved += OnCardMove;
            }
        }

        private void OnCardMove(params object[] vals)
        {
            if (!Utilities.ValidateParam<Card>(this, vals[0]))
                return;
            Card triggeringCard = (Card)vals[0];
            foreach (Vector2Int adjacent in _adjacents)
            {
                if (card.FieldPosition + adjacent == triggeringCard.FieldPosition && card.Controller == triggeringCard.Controller)
                {
                    InvokeTrigger(triggeringCard);
                }
            }
        }
    }
}