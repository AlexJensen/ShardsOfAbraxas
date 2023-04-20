using Abraxas.Events;
using Abraxas.Game;
using System.Collections;
using Zenject;

namespace Abraxas.GameStates
{
    public class CombatState : GameState
    {
        #region Dependency Injections
        public CombatState(IGameManager gameManager, IEventManager eventManager) : base(gameManager, eventManager){}
        public class Factory : PlaceholderFactory<CombatState>{}
        #endregion

        #region Methods
        public override GameStates NextState()
        {
            return GameStates.AfterCombat;
        }

        public override IEnumerator OnEnterState()
        {
            yield return base.OnEnterState();
            yield return gameManager.BeginNextGameState();
        }

        public override IEnumerator OnExitState()
        {
            yield return base.OnExitState();
        }
        #endregion
    }
}