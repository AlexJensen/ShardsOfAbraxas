using System.Collections;

namespace Abraxas.Players
{
    public interface IPlayerManager
    {
        Players ActivePlayer { get;}

        IEnumerator ToggleActivePlayer();
        void ModifyPlayerHealth(Players player, int amount);
    }
}
