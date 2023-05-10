using System.Collections.Generic;
using UnityEngine;

namespace Abraxas.Stones
{
    public abstract class TriggerStone : IStoneController
    {
        [SerializeField]
        List<EffectStone> _effects;

        public abstract int Cost { get; set; }
        public abstract string Info { get; set; }
        public abstract StoneType StoneType { get; set; }

        public void InvokeTrigger(params object[] vals)
        {
            foreach (EffectStone effect in _effects)
            {
                effect.TriggerEffect(vals);
            }
        }
    }
}