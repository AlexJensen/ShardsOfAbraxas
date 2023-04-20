using System.Collections;
using UnityEngine;
using Zenject;

namespace Abraxas.GameStates
{

    public class GameStateManager :  IGameStateManager
    {
        #region Dependencies
        readonly IGameStateFactory _stateFactory;

        public GameStateManager(IGameStateFactory stateFactory)
        {
            _stateFactory = stateFactory;
        }
        #endregion

        #region Fields
        GameState _state = null;
        #endregion

        #region Methods
        public IEnumerator SwitchGameStateTo(GameStates state)
        {
            yield return _state?.OnExitState();
            _state = _stateFactory.CreateState(state);
            yield return _state?.OnEnterState();
        }

        public IEnumerator BeginNextGameState()
        {
            yield return SwitchGameStateTo(_state.NextState());
        }
        #endregion
    }
}
