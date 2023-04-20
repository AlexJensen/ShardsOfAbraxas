using System.Collections;


namespace Abraxas.GameStates
{
    public interface IGameStateManager
    {
        IEnumerator SwitchToState(GameStates state);
        IEnumerator BeginNextGameState();
    }
}
