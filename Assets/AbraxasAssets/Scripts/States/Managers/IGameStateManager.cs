using System.Collections;


namespace Abraxas.GameStates
{
    /// <summary>
    /// IGameStateManager is an interface for managing game states.
    /// </summary>
    public interface IGameStateManager
    {
        GameState State { get; }
        IEnumerator BeginNextGameState();
        void RequestNextGameState();
        IEnumerator InitializeState(GameStates gameNotStarted);
    }
}
