using System.Collections;


namespace Abraxas.GameStates
{
    public interface IGameStateManager
    {
        GameState State { get; }

        IEnumerator SwitchGameStateTo(GameStates state);
        IEnumerator BeginNextGameState();
        IEnumerator RequestNextGameState();
    }
}
