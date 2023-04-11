using Abraxas.Behaviours.Game;
using System.Collections;
using Zenject;

namespace Abraxas.Scripts.States
{
    public class EndState : GameState
    {
        public override GameStates NextState()
        {
            return GameStates.Beginning;
        }

        public EndState(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public class Factory : PlaceholderFactory<EndState>
        {
        }

        public override IEnumerator OnEnterState()
        {
            gameManager.BeginNextGameState();
            yield break;
        }

        public override IEnumerator OnExitState()
        {
            gameManager.SwitchActivePlayer();
            yield break;
        }
    }
}