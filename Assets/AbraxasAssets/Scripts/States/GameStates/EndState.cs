using Abraxas.Events;
using Abraxas.Game.Managers;
using Abraxas.Players.Managers;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Abraxas.GameStates
{
    public class EndState : GameState
    {
        #region Dependencies
        readonly IGameStateManager _gameStateManager;
        readonly IPlayerManager _playerManager;
        readonly NetworkManager _networkManager;
        [Inject]
        public EndState(IGameManager gameManager, IGameStateManager gameStateManager, IPlayerManager playerManager, IEventManager eventManager) : base (gameManager, eventManager)
        {
            _gameStateManager = gameStateManager;
            _playerManager = playerManager;
            _networkManager = NetworkManager.Singleton;
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
            if (_networkManager.IsServer)
            {
                yield return _playerManager.SetActivePlayer(_playerManager.ActivePlayer == Players.Players.Player1 ? Players.Players.Player2 : Players.Players.Player1);
                yield return new WaitForSeconds(.1f);
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