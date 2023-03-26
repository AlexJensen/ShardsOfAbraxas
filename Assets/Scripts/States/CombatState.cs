using System.Collections;

public class CombatState : State
{
    public CombatState(Game game) : base(game)
    {
    }

    public override State NextState()
    {
        return new AfterCombatState(game);
    }

    public override IEnumerator OnEnterState()
    {
        yield return Field.Instance.StartCombat();
        game.BeginNextGameState();
    }

    public override IEnumerator OnExitState()
    {
        yield break;
    }
}
