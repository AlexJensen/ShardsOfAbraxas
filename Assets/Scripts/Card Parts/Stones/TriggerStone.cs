using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TriggerStone : Stone
{
    [SerializeField]
    List<EffectStone> effects;

    public void InvokeTrigger(params object[] vals)
    {
        foreach (EffectStone effect in effects)
        {
            effect.TriggerEffect(vals);
        }
    }
}
