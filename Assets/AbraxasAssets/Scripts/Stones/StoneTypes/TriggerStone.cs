using System.Collections.Generic;
using UnityEngine;

namespace Abraxas.Behaviours.Stones
{
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
}