using Abraxas.Health.Controllers;

namespace Abraxas.Health.Managers
{
    public interface IPlayerHealthManager
    {
        IPlayerHealthController GetPlayerHealth(Players.Players player);
        void ModifyPlayerHealth(Players.Players player, int amount);
        void AddPlayerHealth(IPlayerHealthController playerHealthController);
    }
}
