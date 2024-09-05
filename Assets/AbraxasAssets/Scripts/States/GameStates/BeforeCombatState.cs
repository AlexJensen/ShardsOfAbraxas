using Abraxas.Events.Managers;
using Abraxas.Games.Managers;
using System.Collections;
using Zenject;

namespace Abraxas.GameStates
{
    /// <summary>
    /// BeforeCombatState is the first main phase state before combat.
    /// </summary>
    public class BeforeCombatState : GameState
    {
        #region Dependencies
        
        readonly IGameStateManager _gameStateManager;

        [Inject]
        public BeforeCombatState(IGameManager gameManager, IGameStateManager gameStateManager, IEventManager eventManager) : base(gameManager, eventManager) 
        {
            _gameStateManager = gameStateManager;
        }

        public class Factory : PlaceholderFactory<BeforeCombatState> { }
        #endregion

        #region Properties
        public override GameStates CurrentState => GameStates.BeforeCombat;
        #endregion

        #region Methods
        public override GameStates NextState()
        {
            return GameStates.Combat;
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