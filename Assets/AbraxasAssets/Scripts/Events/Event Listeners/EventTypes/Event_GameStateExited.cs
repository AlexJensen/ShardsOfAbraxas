using Abraxas.GameStates;

namespace Abraxas.Events
{
    public class Event_GameStateExited : IEvent<GameState>
    {
        #region Properties
        public GameState Data { get; set; }
        #endregion

        #region Methods
        public Event_GameStateExited(GameState state)
        {
            Data = state;
        }
        #endregion
    }
}
