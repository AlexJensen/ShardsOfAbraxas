using Abraxas.StatusEffects;
using Abraxas.StatusEffects.Types;

namespace Abraxas.Passives
{
    internal class Passive_Repeater : PassiveStone
    {
        #region Fields
        public StatusEffect_Repeater _statusEffect = new();
        #endregion

        #region Properties

        #endregion
        public override IStatusEffect StatusEffect { get => _statusEffect; }
    }
}
