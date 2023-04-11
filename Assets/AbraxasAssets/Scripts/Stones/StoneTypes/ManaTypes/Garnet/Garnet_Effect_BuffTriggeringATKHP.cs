using Abraxas.Behaviours.Cards;
using Abraxas.Behaviours.Data;
using Abraxas.Behaviours.Events;
using Abraxas.Behaviours.Status;
using Abraxas.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Abraxas.Behaviours.Stones
{
    public class Garnet_Effect_BuffTriggeringATKHP : EffectStone
    {
        List<StatBuff> _buffs;

        public override int Cost { get; set; }
        public override string Info { get; set; }
        public override StoneData.StoneType StoneType { get; set; }

        protected void Awake()
        {
            Cost = 4;
            StoneType = StoneData.StoneType.GARNET;
            Info = "Give the triggering packet +1/+1/+0 until this packet is destroyed.";
            _buffs = new List<StatBuff>();
        }

        private void OnEnable()
        {
            EventManager.Instance.OnCardDestroyed += ClearEffect;
        }

        private void OnDisable()
        {
            EventManager.Instance.OnCardDestroyed -= ClearEffect;
        }


        public override void TriggerEffect(object[] vals)
        {
            if (!Utilities.ValidateParam<Card>(this, vals[0]))
                return;

            Card triggeringCard = (Card)vals[0];

            _buffs.Add(new StatBuff(triggeringCard.GetComponent<StatBlock>(), new Vector3Int(1, 1, 0)));
        }

        public void ClearEffect(object[] vals)
        {
            foreach (StatBuff buff in _buffs)
            {
                buff.Clear(vals);
            }
            _buffs.Clear();
        }
    }
}