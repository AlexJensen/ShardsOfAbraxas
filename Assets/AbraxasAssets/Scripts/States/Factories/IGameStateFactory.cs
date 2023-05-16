namespace Abraxas.GameStates
{
    public interface IGameStateFactory
    {
        public GameState CreateState(GameStates state);
    }
}
