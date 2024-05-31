using Abraxas.Health.Controllers;

namespace Abraxas.Health.Managers
{

    /// <summary>
    /// IPlayerHealthManager is an interface for managing player health.
    /// </summary>
    public interface IPlayerHealthManager
    {
        IPlayerHealthController GetPlayerHealth(Players.Players player);
        void ModifyPlayerHealth(Players.Players player, int amount);
        void AddPlayerHealth(IPlayerHealthController playerHealthController);
    }
}
