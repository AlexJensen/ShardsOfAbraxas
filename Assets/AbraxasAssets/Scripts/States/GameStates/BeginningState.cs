using Abraxas.Events;
using Abraxas.Game;
using System.Collections;
using Zenject;

namespace Abraxas.GameStates
{
    public class BeginningState : GameState
    {
        #region Dependencies
        [Inject]
        public BeginningState(IGameManager gameManager, IEventManager eventManager) : base(gameManager, eventManager){}
        public class Factory : PlaceholderFactory<BeginningState>{}
        #endregion

        #region Methods
        public override GameStates NextState()
        {
            return GameStates.BeforeCombat;
        }
        public override IEnumerator OnEnterState()
        {
            yield return base.OnEnterState();
            yield return gameManager.DrawStartOfTurnCardsForActivePlayer();
            yield return gameManager.GenerateStartOfTurnManaForActivePlayer();
            yield return gameManager.BeginNextGameState();
        }
        public override IEnumerator OnExitState()
        {
            yield return base.OnExitState();
        }
        #endregion
    }
}