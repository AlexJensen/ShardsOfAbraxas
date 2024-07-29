using Abraxas.Cards.Controllers;

namespace Abraxas.StatusEffects
{
    abstract class StatusEffect : IStatusEffect
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public abstract void ApplyEffect(ICardController card);

        public abstract ICardController GetDecorator(ICardController card);
    }
}
