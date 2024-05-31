namespace Abraxas.GameStates
{
    /// <summary>
    /// IGameStateFactory is an interface for creating game states.
    /// </summary>
    public interface IGameStateFactory
    {
        public GameState CreateState(GameStates state);
    }
}
