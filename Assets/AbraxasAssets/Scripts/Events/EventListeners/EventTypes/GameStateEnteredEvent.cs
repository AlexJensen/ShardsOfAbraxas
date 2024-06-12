using Abraxas.GameStates;

namespace Abraxas.Events
{
    /// <summary>
    /// GameStateEnteredEvent is an event that is triggered when a game state is entered.
    /// </summary>
    public class GameStateEnteredEvent : IEvent
    {
        #region Properties
        public GameState State { get; }
        #endregion

        #region Methods
        public GameStateEnteredEvent(GameState state)
        {
            State = state;
        }
        #endregion
    }
}
