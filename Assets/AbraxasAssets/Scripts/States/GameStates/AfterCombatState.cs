using Abraxas.AI.Managers;
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
        readonly IAIManager _aiManager;

        [Inject]
        public AfterCombatState(IGameManager gameManager, IGameStateManager gameStateManager, IEventManager eventManager, IAIManager aiManager) : base(gameManager, eventManager)
        {
            _gameStateManager = gameStateManager;
            _aiManager = aiManager;
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
            else
            {
                yield return _aiManager.DeterminePlay();
            }
        }

        public override IEnumerator OnExitState()
        {
            yield return base.OnExitState();
        }
        #endregion
    }
}