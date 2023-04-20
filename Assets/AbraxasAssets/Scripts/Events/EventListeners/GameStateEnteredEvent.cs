using Abraxas.GameStates;

namespace Abraxas.Events
{
    public class GameStateEnteredEvent
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
