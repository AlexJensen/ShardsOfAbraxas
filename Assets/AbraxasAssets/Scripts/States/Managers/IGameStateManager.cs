using System.Collections;


namespace Abraxas.GameStates
{
    public interface IGameStateManager
    {
        GameState State { get; }
        IEnumerator BeginNextGameState();
        IEnumerator RequestNextGameState();
        IEnumerator InitializeState(GameStates gameNotStarted);
    }
}
