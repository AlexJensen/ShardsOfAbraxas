using Player = Abraxas.Players.Players;

namespace Abraxas.Health.Controllers
{
    /// <summary>
    /// IPlayerHealthController is an interface for controlling player health.
    /// </summary>
    public interface IPlayerHealthController
    {
        int HP { get; set; }
        int MaxHP { get; set; }
        Player Player { get; set; }
    }
}
