using Abraxas.GameStates;

namespace Abraxas.Events
{
    /// <summary>
    /// GameStateEnteredEvent is an event that is triggered when a game state is entered.
    /// </summary>
    public class Event_GameStateEntered : IEvent
    {
        #region Properties
        public GameState State { get; set; }
        #endregion

        #region Methods
        public Event_GameStateEntered(GameState state)
        {
            State = state;
        }
        #endregion
    }
}
