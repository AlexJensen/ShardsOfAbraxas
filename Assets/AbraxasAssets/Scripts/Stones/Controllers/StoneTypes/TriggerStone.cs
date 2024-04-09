using Abraxas.Stones.Controllers;
using System.Collections.Generic;
using UnityEngine;

namespace Abraxas.Stones
{
    public abstract class TriggerStone : StoneController
    {
        [SerializeField]
        List<EffectStone> _effects;

        public void InvokeTrigger(params object[] vals)
        {
            foreach (EffectStone effect in _effects)
            {
                effect.TriggerEffect(vals);
            }
        }
    }
}