using System.Collections;

namespace Abraxas.Players
{
    public interface IPlayerManager
    {
        Players ActivePlayer { get;}
        Players LocalPlayer { get; }

        void RegisterLocalPlayer(Players player);
        IEnumerator ToggleActivePlayer();
    }
}
