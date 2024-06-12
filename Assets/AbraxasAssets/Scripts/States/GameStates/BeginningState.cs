using Abraxas.Events.Managers;
using Abraxas.Games.Managers;
using System.Collections;
using Zenject;

namespace Abraxas.GameStates
{
    /// <summary>
    /// BeginningState is the start of the turn.
    /// </summary>
    public class BeginningState : GameState
    {
        #region Dependencies
        readonly IGameStateManager _gameStateManager;
        [Inject]
        public BeginningState(IGameManager gameManager, IGameStateManager gameStateManager, IEventManager eventManager) : base(gameManager, eventManager)
        {
            _gameStateManager = gameStateManager;
        }
        public class Factory : PlaceholderFactory<BeginningState> { }
        #endregion

        #region Properties
        public override GameStates CurrentState => GameStates.Beginning;
        #endregion

        #region Methods
        public override GameStates NextState()
        {
            return GameStates.BeforeCombat;
        }
        public override IEnumerator OnEnterState()
        {
            yield return base.OnEnterState();
            yield return gameManager.GenerateStartOfTurnManaForActivePlayer();
            yield return gameManager.DrawStartOfTurnCardsForActivePlayer();
            yield return _gameStateManager.BeginNextGameState();
        }
        public override IEnumerator OnExitState()
        {
            yield return base.OnExitState();
        }
        #endregion
    }
}