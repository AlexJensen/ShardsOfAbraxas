using Abraxas.Cards.Controllers;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Zenject;

namespace Abraxas.StatusEffects
{
    abstract class StatusEffect : IStatusEffect
    {
        public abstract void ApplyEffect(ICardController card);
        public abstract ICardController GetDecorator(ICardController card, ICardModel model, ICardView view, DiContainer container);
    }
}
