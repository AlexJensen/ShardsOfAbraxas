using System;
using System.Collections;

public class BeginningState : State
{
    public BeginningState(Game game) : base(game)
    {
    }

    public override State NextState()
    {
        return new BeforeCombatState(game);
    }

    public override IEnumerator OnEnterState()
    {
        Game.Instance.GenerateManaForCurrentPlayer();
        yield return Game.Instance.DrawCardsForCurrentPlayer(1);
        game.BeginNextGameState();
    }

    public override IEnumerator OnExitState()
    {
        yield break;
    }
}
