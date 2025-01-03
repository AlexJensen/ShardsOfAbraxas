using Abraxas.Cards.Controllers;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using System.Collections;

class AdvanceFireDecorator : CardDecorator
{
    public AdvanceFireDecorator(ICardControllerInternal innerController, ICardModel model, ICardView view)
        : base(innerController, model, view) { }


    public override IEnumerator PreCombat()
    {
        yield return base.PreCombat();
        EnablePreMovementRangedAttack = true;
    }
}
