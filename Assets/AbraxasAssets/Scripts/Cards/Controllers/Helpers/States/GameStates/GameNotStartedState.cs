using Abraxas.Events;
using Abraxas.Game.Managers;
using System.Collections;
using Unity.Netcode;

using Zenject;
using Abraxas.Network.Managers;

namespace Abraxas.GameStates
{
    public class GameNotStartedState : GameState
    {
        #region Dependencies
        readonly NetworkManager _networkManager;
        readonly DebugNetworkManager _debugNetworkManager;
        readonly IGameStateManager _gameStateManager;
        [Inject]
        public GameNotStartedState(IGameManager gameManager, IGameStateManager gameStateManager, IEventManager eventManager, DebugNetworkManager debugNetworkManager) :base(gameManager, eventManager)
        {
            _gameStateManager = gameStateManager;
            _networkManager = NetworkManager.Singleton;
            _debugNetworkManager = debugNetworkManager;
        }

        public class Factory : PlaceholderFactory<GameNotStartedState>
        {
        }
        #endregion

        #region Properties
        public override GameStates CurrentState => GameStates.GameNotStarted;
        #endregion

        #region Methods
        public override GameStates NextState()
        {
            return GameStates.Beginning;
        }

        public override IEnumerator OnEnterState()
        {
            yield return base.OnEnterState();
            if (_debugNetworkManager.isDebugMode)
            {
                yield return _gameStateManager.BeginNextGameState();
            }
            else
            {
                while (!_networkManager.IsServer && !_networkManager.IsClient)
                {
                    yield return null;
                }

                if (_networkManager.IsServer)
                {
                    while (_networkManager.ConnectedClients.Count != 2)
                    {
                        yield return null;
                    }
                    yield return _gameStateManager.BeginNextGameState();
                }
            }
        }

        public override IEnumerator OnExitState()
        {
            yield return base.OnExitState();
            yield return gameManager.StartGame();
        }
        #endregion
    }
}