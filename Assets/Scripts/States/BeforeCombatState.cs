using System.Collections;

public class BeforeCombatState : State
{
    public BeforeCombatState(Game game) : base(game)
    {
    }

    public override State NextState()
    {
        return new CombatState(game);
    }

    public override IEnumerator OnEnterState()
    {
        Events.Instance.BeforeCombatStarted();
        yield break;
    }

    public override IEnumerator OnExitState()
    {
        yield break;
    }
}
