using System.Collections;

public class EndState : State
{
    public EndState(Game game) : base(game)
    {
    }

    public override State NextState()
    {
        return new BeginningState(game);
    }

    public override IEnumerator OnEnterState()
    {
        game.BeginNextGameState();
        yield break;
    }

    public override IEnumerator OnExitState()
    {
        Game.Instance.SwitchActivePlayer();
        yield break;
    }
}
