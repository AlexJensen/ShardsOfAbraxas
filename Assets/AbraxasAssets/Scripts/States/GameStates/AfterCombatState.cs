﻿using Abraxas.Events;
using Abraxas.Game;
using System.Collections;
using Zenject;

namespace Abraxas.GameStates
{
    public class AfterCombatState : GameState
    {
        #region Dependencies
        [Inject]
        public AfterCombatState(IGameManager gameManager, IEventManager eventManager) : base(gameManager, eventManager){}
        public class Factory : PlaceholderFactory<AfterCombatState>{}
        #endregion

        #region Methods
        public override GameStates NextState()
        {
            return GameStates.End;
        }

        public override IEnumerator OnEnterState()
        {
            yield return base.OnEnterState();
        }

        public override IEnumerator OnExitState()
        {
            yield return base.OnExitState();
        }
        #endregion
    }
}