using Abraxas.Cards.Controllers;

namespace Abraxas.StatusEffects
{
    public interface IStatusEffect
    {
        void ApplyEffect(ICardController card);
        ICardController GetDecorator(ICardController card);
    }
}
