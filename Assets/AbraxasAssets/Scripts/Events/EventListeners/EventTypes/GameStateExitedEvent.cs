using Abraxas.GameStates;

namespace Abraxas.Events
{
    public class GameStateExitedEvent : IEvent
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
