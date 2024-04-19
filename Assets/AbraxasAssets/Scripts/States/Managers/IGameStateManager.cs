using System.Collections;


namespace Abraxas.GameStates
{
    public interface IGameStateManager
    {
        GameState State { get; }
        IEnumerator BeginNextGameState();
        void RequestNextGameState();
        IEnumerator InitializeState(GameStates gameNotStarted);
    }
}
