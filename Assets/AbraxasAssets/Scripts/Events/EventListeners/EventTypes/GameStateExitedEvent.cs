using Abraxas.GameStates;

namespace Abraxas.Events
{
    public class GameStateExitedEvent
    {
        #region Properties
        public GameState State { get; }
        #endregion

        #region Methods
        public GameStateExitedEvent(GameState state)
        {
            State = state;
        }
        #endregion
    }
}
