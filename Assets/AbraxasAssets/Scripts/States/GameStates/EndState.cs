﻿using Abraxas.Events;
using Abraxas.Game;
using Abraxas.Players;
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
            _playerManager.ToggleActivePlayer();
        }
        #endregion
    }
}