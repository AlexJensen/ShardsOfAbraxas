using Abraxas.Cards.Controllers;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using System.Collections;

class HardenedDecorator : CardDecorator
{
    public HardenedDecorator(ICardControllerInternal innerController, ICardModel model, ICardView view)
        : base(innerController, model, view) { }

    public override IEnumerator TakeDamage(int amount)
    {
        if (amount >= StatBlock.Stats.DEF)
        {
           yield return base.TakeDamage(amount);
        }
    }
}
