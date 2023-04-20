using Abraxas.Network;
using Abraxas.Game;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using Zenject;
using Abraxas.Events;

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
                yield return new WaitForSeconds(1.0f);
            }
            else
            {
                while (!_networkManager.IsServer)
                {
                    Debug.Log(_networkManager.IsServer);
                    yield return null;
                }
                while (_networkManager.ConnectedClients.Count != 2)
                {
                    Debug.Log(_networkManager.ConnectedClients.Count);
                    yield return null;
                }
            }
            yield return _gameStateManager.BeginNextGameState();
        }

        public override IEnumerator OnExitState()
        {
            yield return base.OnExitState();
            yield return gameManager.StartGame();
        }
        #endregion
    }
}