using Abraxas.Cards.Controllers;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;

class JammerDecorator : DefaultBehaviorDecorator
{
    public JammerDecorator(ICardControllerInternal innerController, ICardModel model, ICardView view)
        : base(innerController, model, view) { }

    public override bool CanBeAttackedRanged()
    {
        return false;
    }
}
