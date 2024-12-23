using Abraxas.Cards.Controllers;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.Zones.Fields.Controllers;
using System.Collections;

class AdvanceFireDecorator : CardDecorator
{
    public AdvanceFireDecorator(ICardControllerInternal innerController, ICardModel model, ICardView view)
        : base(innerController, model, view) { }


    public override IEnumerator PreMovementAction(IFieldController field)
    {
        Aggregator.EnablePreMovementRangedAttack = true;
        yield return base.PreMovementAction(field);
    }

    public override IEnumerator PostMovementAction(IFieldController field)
    {
        Aggregator.EnablePostMovementRangedAttack = false;
        return base.PostMovementAction(field);
    }
}
