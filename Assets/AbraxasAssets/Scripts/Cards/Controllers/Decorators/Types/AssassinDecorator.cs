using System.Collections;
using Abraxas.Cards.Controllers;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;

class AssassinDecorator : CardDecorator
{
    public AssassinDecorator(ICardControllerInternal innerController, ICardModel model, ICardView view)
        : base(innerController, model, view) { }

    public override IEnumerator DealDamage(ICardController opponent, int amount)
    {
        var originalDef = opponent.StatBlock.Stats.DEF;
        yield return base.DealDamage(opponent, amount);
        var newDef = opponent.StatBlock.Stats.DEF;
        if (newDef < originalDef && newDef > 0)
        {
            var stats = opponent.StatBlock.Stats;
            stats.DEF = 0;
            opponent.StatBlock.Stats = stats;
            yield return opponent.CheckDeath();
        }
    }
}
