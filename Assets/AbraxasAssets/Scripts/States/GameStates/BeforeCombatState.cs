using Abraxas.Behaviours.Events;
using Abraxas.Behaviours.Game;
using System.Collections;
using Zenject;

namespace Abraxas.Scripts.States
{
    public class BeforeCombatState : GameState
    {
        public override GameStates NextState()
        {
            return GameStates.Combat;
        }

        public BeforeCombatState(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public class Factory : PlaceholderFactory<BeforeCombatState>
        {

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