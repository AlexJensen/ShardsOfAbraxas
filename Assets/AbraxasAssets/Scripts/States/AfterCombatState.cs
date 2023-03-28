using Abraxas.Behaviours.Game;
using System.Collections;

namespace Abraxas.Scripts.States
{
    public class AfterCombatState : State
    {
        public AfterCombatState(GameManager game) : base(game)
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
}