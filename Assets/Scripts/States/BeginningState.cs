using System;
using System.Collections;
using UnityEngine;

/// <summary>
/* Beginning of the turn state, leads directly to before combat state once all beginning of turn effects have completed.*/
/// </summary>
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
        Game.Instance.GenerateManaForCurrentPlayer();
        yield return Game.Instance.DrawCardsForCurrentPlayer(1);


        game.BeginNextGameState();
    }

    public override IEnumerator OnExitState()
    {
        yield break;
    }
}
