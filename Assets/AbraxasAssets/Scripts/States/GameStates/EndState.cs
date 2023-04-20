using Abraxas.Events;
using Abraxas.Game;
using System.Collections;
using Zenject;

namespace Abraxas.GameStates
{
    public class EndState : GameState
    {
        #region Dependencies
        [Inject]
        public EndState(IGameManager gameManager, IEventManager eventManager) : base (gameManager, eventManager){}
        public class Factory : PlaceholderFactory<EndState>{}
        #endregion

        #region Methods
        public override GameStates NextState()
        {
            return GameStates.Beginning;
        }

        public override IEnumerator OnEnterState()
        {
            yield return base.OnEnterState();
            yield return gameManager.BeginNextGameState();
        }

        public override IEnumerator OnExitState()
        {
            yield return base.OnExitState();
            gameManager.ToggleActivePlayer();
        }
        #endregion
    }
}