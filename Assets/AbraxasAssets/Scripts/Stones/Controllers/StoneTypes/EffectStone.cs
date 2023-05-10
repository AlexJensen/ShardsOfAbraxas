namespace Abraxas.Stones
{
    public abstract class EffectStone : IStoneController
    {
        public abstract int Cost { get; set; }
        public abstract string Info { get; set; }
        public abstract StoneType StoneType { get; set; }

        public abstract void TriggerEffect(object[] vals);
    }
}