using Abraxas.Cards.Controllers;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using System.Collections;

class SereneDecorator : CardDecorator
{
    public SereneDecorator(ICardControllerInternal innerController, ICardModel model, ICardView view)
        : base(innerController, model, view) { }

    public override IEnumerator Fight(ICardController opponent)
    {
        CanFight = false;
        yield return base.Fight(opponent);
    }

}
