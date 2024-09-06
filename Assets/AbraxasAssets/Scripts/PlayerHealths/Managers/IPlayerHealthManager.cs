using Abraxas.Health.Controllers;
using System.Collections;

namespace Abraxas.Health.Managers
{

    /// <summary>
    /// IPlayerHealthManager is an interface for managing player health.
    /// </summary>
    public interface IPlayerHealthManager
    {
        IPlayerHealthController GetPlayerHealth(Players.Players player);
        IEnumerator ModifyPlayerHealth(Players.Players player, int amount);
        void AddPlayerHealthController(IPlayerHealthController playerHealthController);
    }
}
