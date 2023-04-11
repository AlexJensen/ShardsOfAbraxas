using Abraxas.Behaviours.Game;
using Abraxas.Behaviours.Network;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Abraxas.Scripts.States
{
    /// <summary>
    /// State to wait in while players are connecting.
    /// </summary>
    public class GameNotStartedState : GameState
    {
        private readonly NetworkManager _networkManager;
        private readonly DebugNetworkManager _debugNetworkManager;
        public GameNotStartedState(GameManager gameManager, DebugNetworkManager debugNetworkManager)
        {
            _networkManager = NetworkManager.Singleton;
            _debugNetworkManager = debugNetworkManager;
            this.gameManager = gameManager;
        }

        public class Factory : PlaceholderFactory<GameNotStartedState>
        {
        }

        public override GameStates NextState()
        {
            return GameStates.Beginning;
        }

        public override IEnumerator OnEnterState()
        {
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
            gameManager.BeginNextGameState();
        }

        public override IEnumerator OnExitState()
        {
            yield return gameManager.StartGame();
        }
    }
}