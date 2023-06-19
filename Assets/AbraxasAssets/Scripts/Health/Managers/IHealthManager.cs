using Abraxas.Health.Controllers;

namespace Abraxas
{
    public interface IHealthManager
    {
        IPlayerHealthController GetPlayerHealth(Players.Players player);
        void ModifyPlayerHealth(Players.Players player, int amount);
    }
}
