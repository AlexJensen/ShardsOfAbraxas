using Abraxas.Cards.Controllers;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Zenject;

namespace Abraxas.StatusEffects
{
    public interface IStatusEffect
    {
        void ApplyEffect(ICardController card);
        ICardController GetDecorator(ICardController card, ICardModel model, ICardView view, DiContainer container);
    }
}
