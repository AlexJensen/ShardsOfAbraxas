using Abraxas.Cards.Controllers;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Zenject;

namespace Abraxas.StatusEffects.Types
{
    internal class StatusEffect_Hardened : StatusEffect
    {
        public override void ApplyEffect(ICardController card)
        {
            
        }

        public override ICardController GetDecorator(ICardController card, ICardModel model, ICardView view, DiContainer container)
        {
            var internalController = card as ICardControllerInternal;
            return container.Instantiate<HardenedDecorator>(new object[] { internalController, model, view });
        }

    }
}
