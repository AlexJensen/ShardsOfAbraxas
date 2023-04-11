using Abraxas.Behaviours.Events;
using Abraxas.Behaviours.Game;
using System.Collections;
using Zenject;

namespace Abraxas.Scripts.States
{
    public class BeginningState : GameState
    {
        public override GameStates NextState()
        {
            return GameStates.BeforeCombat;
        }

        public BeginningState(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public class Factory : PlaceholderFactory<BeginningState>
        {
        }

        public override IEnumerator OnEnterState()
        {
            gameManager.GenerateManaForActivePlayer();
            yield return gameManager.DrawCardsForActivePlayer(1);
            EventManager.Instance.BeginningStateStarted();
            gameManager.BeginNextGameState();
        }

        public override IEnumerator OnExitState()
        {
            yield break;
        }
    }
}