using System.Collections;

namespace Abraxas.Players.Managers
{
    /// <summary>
    /// IPlayerManager is an interface for managing players.
    /// </summary>
    public interface IPlayerManager
    {
        Players ActivePlayer { get; }
        Players LocalPlayer { get; }

        void RegisterLocalPlayer(Players player);
        IEnumerator SetActivePlayer(Players player);
    }
}
