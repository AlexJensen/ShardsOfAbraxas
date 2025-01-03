using Abraxas.StatusEffects;
using Abraxas.StatusEffects.Types;

namespace Abraxas.Passives
{
    internal class Passive_Stonewall : PassiveStone
    {
        #region Fields
        public StatusEffect_Stonewall _statusEffect = new();
        #endregion

        #region Properties
        public override IStatusEffect StatusEffect { get => _statusEffect; }
        #endregion
    }
}
