using Abraxas.Cards.Controllers;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.StatusEffects.Types;
using System.Collections;

class HardenedDecorator : CardDecorator
{
    public HardenedDecorator(ICardControllerInternal innerController, ICardModel model, ICardView view)
        : base(innerController, model, view) { }

    public override IEnumerator TakeDamage(ICardController source, int amount)
    {
        if (amount >= StatBlock.Stats.DEF || source.RequestHasStatusEffect<StatusEffect_Assassin>())
        {
           yield return base.TakeDamage(source, amount);
        }
        else
        {
            yield return base.TakeDamage(source, 0);
        }
    }
}
