using Abraxas.Behaviours.Events;
using Abraxas.Behaviours.Game;
using System.Collections;

namespace Abraxas.Scripts.States
{
    public class BeforeCombatState : State
    {
        public BeforeCombatState(GameManager game) : base(game)
        {
        }

        public override State NextState()
        {
            return new CombatState(game);
        }

        public override IEnumerator OnEnterState()
        {
            EventManager.Instance.BeforeCombatStarted();
            yield break;
        }

        public override IEnumerator OnExitState()
        {
            yield break;
        }
    }
}