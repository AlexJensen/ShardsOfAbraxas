using Abraxas.Cards.Controllers;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using System.Collections;

class DefensiveDecorator : CardDecorator
{
    public DefensiveDecorator(ICardControllerInternal innerController, ICardModel model, ICardView view)
        : base(innerController, model, view) { }

    public override IEnumerator Attack(ICardController opponent, bool ranged)
    {
        var statBlock = StatBlock.Stats;
        var oldATK = statBlock.ATK;
        statBlock.ATK = statBlock.DEF;
        StatBlock.Stats = statBlock;
        yield return base.Attack(opponent, ranged);
        statBlock.ATK = oldATK;
        statBlock.DEF = StatBlock.Stats.DEF;
        StatBlock.Stats = statBlock;
    }
}
