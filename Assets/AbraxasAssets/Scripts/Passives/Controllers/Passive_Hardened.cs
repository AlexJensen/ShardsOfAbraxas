using Abraxas.StatusEffects;
using Abraxas.StatusEffects.Types;

namespace Abraxas.Passives
{
    internal class Passive_Hardened : PassiveStone
    {
        #region Fields
        public StatusEffect_Hardened _statusEffect = new();
        #endregion

        #region Properties

        #endregion
        public override IStatusEffect StatusEffect { get => _statusEffect; }
    }
}
