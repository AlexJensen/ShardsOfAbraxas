using Abraxas.Cards.Managers;
using Abraxas.Events.Managers;
using Abraxas.Games.Managers;
using System.Collections;
using Zenject;

namespace Abraxas.GameStates
{
    /// <summary>
    /// AfterCombatState is the second main phase state that directly follows combat.
    /// </summary>
    public class AfterCombatState : GameState
    {
        #region Dependencies
        readonly IGameStateManager _gameStateManager;

        [Inject]
        public AfterCombatState(IGameManager gameManager, IGameStateManager gameStateManager, IEventManager eventManager) : base(gameManager, eventManager)
        {
            _gameStateManager = gameStateManager;
        }
        public class Factory : PlaceholderFactory<AfterCombatState> { }
        #endregion

        #region Properties
        public override GameStates CurrentState => GameStates.AfterCombat;
        #endregion

        #region Methods
        public override GameStates NextState()
        {
            return GameStates.End;
        }

        public override IEnumerator OnEnterState()
        {
            yield return base.OnEnterState();
            if (!gameManager.IsAnyPlayerInputAvailable())
            {
                yield return _gameStateManager.BeginNextGameState();
            }
        }

        public override IEnumerator OnExitState()
        {
            yield return base.OnExitState();
        }
        #endregion
    }
}