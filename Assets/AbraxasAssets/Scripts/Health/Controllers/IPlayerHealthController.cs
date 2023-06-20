using Player = Abraxas.Players.Players;

namespace Abraxas.Health.Controllers
{
    public interface IPlayerHealthController
    {
        int HP { get; set; }
        int MaxHP { get; set; }
        Player Player { get; set; }
    }
}
