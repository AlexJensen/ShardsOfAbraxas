using System.Collections;


namespace Abraxas.GameStates
{
    public interface IGameStateManager
    {
        IEnumerator SwitchGameStateTo(GameStates state);
        IEnumerator BeginNextGameState();
    }
}
