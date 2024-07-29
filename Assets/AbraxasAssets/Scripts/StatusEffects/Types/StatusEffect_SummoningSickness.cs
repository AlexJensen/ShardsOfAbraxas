using Abraxas.Cards.Controllers;

namespace Abraxas.StatusEffects.Types
{
    internal class StatusEffect_SummoningSickness : StatusEffect
    {
        public override void ApplyEffect(ICardController card)
        {
            
        }

        public override ICardController GetDecorator(ICardController card)
        {
            return new SummoningSicknessDecorator(card);
        }
    }
}
