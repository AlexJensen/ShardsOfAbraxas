using Abraxas.Stones.Controllers;
using System.Collections;
using System.Collections.Generic;

namespace Abraxas.Stones
{
    public abstract class TriggerStone : StoneController
    {
        List<EffectStone> _effects = new();

        public List<EffectStone> Effects { get => _effects; set => _effects = value; }

        public IEnumerator InvokeTrigger(params object[] vals)
        {
            foreach (EffectStone effect in _effects)
            {
                yield return effect.TriggerEffect(vals);
            }
        }
    }
}