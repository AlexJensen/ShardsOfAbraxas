using Abraxas.Behaviours.Game;
using System.Collections;
using Zenject;

namespace Abraxas.Scripts.States
{
    public class AfterCombatState : GameState
    {
        public override GameStates NextState()
        {
            return GameStates.End;
        }

        public AfterCombatState(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public class Factory : PlaceholderFactory<AfterCombatState>
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