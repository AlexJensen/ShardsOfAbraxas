namespace Abraxas.Players
{
    public interface IPlayerManager
    {
        Players ActivePlayer { get;}

        void ToggleActivePlayer();
        void ModifyPlayerHealth(Players player, int amount);
    }
}
