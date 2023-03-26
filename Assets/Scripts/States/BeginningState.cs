using System;
using System.Collections;
using UnityEngine;

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
        Events.Instance.BeginningStateStarted();
        Game.Instance.GenerateManaForActivePlayer();
        yield return Game.Instance.DrawCardsForActivePlayer(1);
        game.BeginNextGameState();
    }

    public override IEnumerator OnExitState()
    {
        yield break;
    }
}
