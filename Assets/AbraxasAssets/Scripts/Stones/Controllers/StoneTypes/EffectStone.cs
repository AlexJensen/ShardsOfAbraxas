namespace Abraxas.Stones.Controllers
{
    public abstract class EffectStone : StoneController
    {
        public abstract void TriggerEffect(object[] vals);
    }
}