using Abraxas.GameStates;

namespace Abraxas.Events
{
    public class GameStateChangedEvent
    {
        #region Fields
        readonly GameState _state;
        #endregion

        #region Properties
        public GameState State => _state;
        #endregion

        #region Methods
        public GameStateChangedEvent(GameState state)
        {
            _state = state;
        }
        #endregion
    }
}
