﻿using Abraxas.Events;
using Abraxas.Game.Managers;
using System.Collections;
using Unity.Netcode;
using Zenject;

namespace Abraxas.GameStates
{
    public class BeginningState : GameState
    {
        #region Dependencies
        readonly IGameStateManager _gameStateManager;
        [Inject]
        public BeginningState(IGameManager gameManager, IGameStateManager gameStateManager, IEventManager eventManager) : base(gameManager, eventManager)
        {
            _gameStateManager = gameStateManager;
        }
        public class Factory : PlaceholderFactory<BeginningState>{}
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
            yield return gameManager.DrawStartOfTurnCardsForActivePlayer();
            yield return gameManager.GenerateStartOfTurnManaForActivePlayer();
            yield return _gameStateManager.BeginNextGameState();

        }
        public override IEnumerator OnExitState()
        {
            yield return base.OnExitState();
        }
        #endregion
    }
}