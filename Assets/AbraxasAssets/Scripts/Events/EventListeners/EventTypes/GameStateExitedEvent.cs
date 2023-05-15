using Abraxas.GameStates;

namespace Abraxas.Events
{
    public class GameStateExitedEvent
    {
        #region Fields
        readonly GameState _state;
        #endregion

        #region Properties
        public GameState State => _state;
        #endregion

        #region Methods
        public GameStateExitedEvent(GameState state)
        {
            _state = state;
        }
        #endregion
    }
}
