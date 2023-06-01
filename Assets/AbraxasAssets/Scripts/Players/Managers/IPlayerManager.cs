using System.Collections;

namespace Abraxas.Players.Managers
{
    public interface IPlayerManager
    {
        Players ActivePlayer { get;}
        Players LocalPlayer { get; }

        void RegisterLocalPlayer(Players player);
        IEnumerator SetActivePlayer(Players player);
    }
}
