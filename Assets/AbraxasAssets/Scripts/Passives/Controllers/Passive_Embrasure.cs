using Abraxas.StatusEffects;
using Abraxas.StatusEffects.Types;

namespace Abraxas.Passives
{
    internal class Passive_Embrasure : PassiveStone
    {
        #region Fields
        public StatusEffect_Embrasure _statusEffect = new();
        #endregion

        #region Properties
        public override IStatusEffect StatusEffect { get => _statusEffect; }
        #endregion
    }
}
