using System.Collections;
using Player = Abraxas.Players.Players;

namespace Abraxas.Health.Controllers
{
    /// <summary>
    /// IPlayerHealthController is an interface for controlling player health.
    /// </summary>
    public interface IPlayerHealthController
    {
        int HP { get; }
        int MaxHP { get; }
        Player Player { get; set; }

        IEnumerator SetHealth(int health);
        IEnumerator SetMaxHealth(int v);
    }
}
