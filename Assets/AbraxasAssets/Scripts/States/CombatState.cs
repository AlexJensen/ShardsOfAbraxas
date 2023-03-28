using Abraxas.Behaviours.Game;
using Abraxas.Behaviours.Zones.Fields;
using System.Collections;

namespace Abraxas.Scripts.States
{
    public class CombatState : State
    {
        public CombatState(GameManager game) : base(game)
        {
        }

        public override State NextState()
        {
            return new AfterCombatState(game);
        }

        public override IEnumerator OnEnterState()
        {
            yield return FieldManager.Instance.StartCombat();
            game.BeginNextGameState();
        }

        public override IEnumerator OnExitState()
        {
            yield break;
        }
    }
}