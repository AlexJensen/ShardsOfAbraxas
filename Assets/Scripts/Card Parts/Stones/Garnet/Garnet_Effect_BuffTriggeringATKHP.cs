using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garnet_Effect_BuffTriggeringATKHP : EffectStone
{
    List<StatBuff> buffs;

    protected void Awake()
    {
        cost = 4;
        stoneType = StoneData.StoneType.PERIDOT;
        info = "Give the triggering packet +1/+1/+0 until this packet is destroyed.";
        buffs = new List<StatBuff>();
    }

    private void OnEnable()
    {
        Events.Instance.OnCardDestroyed += ClearEffect;
    }

    private void OnDisable()
    {
        Events.Instance.OnCardDestroyed -= ClearEffect;
    }


    public override void TriggerEffect(object[] vals)
    {
        if (!Utilities.ValidateParam<Card>(this, vals[0]))
            return;

        Card triggeringCard = (Card)vals[0];

        buffs.Add(new StatBuff(triggeringCard.GetComponent<StatBlock>(), new Vector3Int(1,1,0)));
    }

    public void ClearEffect(object[] vals)
    {
        foreach (StatBuff buff in buffs)
        {
            buff.Clear(vals);
        }
        buffs.Clear();
    }
}
