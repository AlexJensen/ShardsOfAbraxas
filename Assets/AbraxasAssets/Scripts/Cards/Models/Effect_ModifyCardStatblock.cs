using Abraxas.Stones.Data;
using System.Collections;

namespace Abraxas.Stones.Controllers
{
    public class Effect_ModifyCardStatBlock : EffectStone
    {
        protected override IEnumerator PerformEffect(object[] vals)
        {
            var modification = ((ModifyCardStatBlockData)Model.Data).Modification;
            Card.StatBlock.Stats += modification;
            yield return null;
        }
    }
}