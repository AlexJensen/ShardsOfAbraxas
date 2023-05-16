using Abraxas.Events;
using Abraxas.Game.Managers;
using Abraxas.Players.Managers;
using System.Collections;
using Zenject;

namespace Abraxas.GameStates
{
    public class EndState : GameState
    {
        #region Dependencies
        readonly IGameStateManager _gameStateManager;
        readonly IPlayerManager _playerManager;
        [Inject]
        public EndState(IGameManager gameManager, IGameStateManager gameStateManager, IPlayerManager playerManager, IEventManager eventManager) : base (gameManager, eventManager)
        {
            _gameStateManager = gameStateManager;
            _playerManager = playerManager;
        }
        public class Factory : PlaceholderFactory<EndState>{}
        #endregion

        #region Properties
        public override GameStates CurrentState => GameStates.End;
        #endregion

        #region Methods
        public override GameStates NextState()
        {
            return GameStates.Beginning;
        }

        public override IEnumerator OnEnterState()
        {
            yield return base.OnEnterState();
            yield return _gameStateManager.BeginNextGameState();
        }

        public override IEnumerator OnExitState()
        {
            yield return base.OnExitState();
            yield return _playerManager.ToggleActivePlayer();
        }
        #endregion
    }
}