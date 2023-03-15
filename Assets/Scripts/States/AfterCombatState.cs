using System.Collections;

public class AfterCombatState : State
{
    public AfterCombatState(Game game) : base(game)
    {
    }

    public override State NextState()
    {
        return new EndState(game);
    }

    public override IEnumerator OnEnterState()
    {
        yield break;
    }

    public override IEnumerator OnExitState()
    {
        yield break;
    }
}
