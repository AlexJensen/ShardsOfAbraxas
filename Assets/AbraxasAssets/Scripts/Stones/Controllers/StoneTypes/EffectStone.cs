using System.Collections;

namespace Abraxas.Stones.Controllers
{
    public abstract class EffectStone : StoneController
    {
        public abstract IEnumerator TriggerEffect(object[] vals);
    }
}