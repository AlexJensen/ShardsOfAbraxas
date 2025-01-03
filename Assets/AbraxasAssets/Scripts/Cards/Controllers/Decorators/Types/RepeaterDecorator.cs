using Abraxas.Cards.Controllers;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using System.Collections;

class RepeaterDecorator : CardDecorator
{
    public RepeaterDecorator(ICardControllerInternal innerController, ICardModel model, ICardView view)
        : base(innerController, model, view) { }

    public override IEnumerator Attack(ICardController opponent, bool ranged)
    {
        yield return base.Attack(opponent, ranged);
        HasAttacked = false;
    }

}
