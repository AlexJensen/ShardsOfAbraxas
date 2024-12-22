using Abraxas.StatusEffects;
using Abraxas.StatusEffects.Types;

namespace Abraxas.Passives
{
    internal class Passive_Flight : PassiveStone
    {
        #region Fields
        public StatusEffect_Flight _statusEffect = new();
        #endregion

        #region Properties
        public override IStatusEffect StatusEffect { get => _statusEffect; }
        #endregion
    }
}
