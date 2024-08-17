using Abraxas.GameStates;

namespace Abraxas.Events
{
    public class Event_GameStateExited : IEvent
    {
        #region Properties
        public GameState State { get; set; }
        #endregion

        #region Methods
        public Event_GameStateExited(GameState state)
        {
            State = state;
        }
        #endregion
    }
}
