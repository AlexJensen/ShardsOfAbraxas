using Abraxas.Cards.Controllers;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Zenject;

namespace Abraxas.StatusEffects.Types
{
    internal class StatusEffect_Stonewall : StatusEffect
    {
        public override void ApplyEffect(ICardController card)
        {
            
        }

        public override ICardController GetDecorator(ICardController card, ICardModel model, ICardView view, DiContainer container)
        {
            return container.Instantiate<StonewallDecorator>(new object[] { card, model, view });
        }
    }
}
