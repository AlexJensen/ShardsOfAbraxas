using Abraxas.AI.Managers;
using Abraxas.Events.Managers;
using Abraxas.Games.Managers;
using Abraxas.Players.Managers;
using System.Collections;
using UnityEngine;
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
        readonly IAIManager _aiManager;


        [Inject]
        public BeforeCombatState(IGameManager gameManager, IGameStateManager gameStateManager, IEventManager eventManager, IAIManager aiManager) : base(gameManager, eventManager) 
        {
            _gameStateManager = gameStateManager;
            _aiManager = aiManager;
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