using System.Collections;
using Unity.Netcode;

namespace Abraxas.Players.Managers
{
    public class PlayerManager : NetworkBehaviour, IPlayerManager
    {
        #region Fields
        readonly NetworkVariable<Players> _activePlayer = new(Players.Player1);
        Players _localPlayer;
        #endregion

        #region Properties
        public Players ActivePlayer => _activePlayer.Value;
        public Players LocalPlayer => _localPlayer;
        #endregion

        #region Methods
        public IEnumerator SetActivePlayer(Players player)
        {
            if (!IsServer) yield break;
            _activePlayer.Value = player;
            if (IsHost) _localPlayer = player;

        }

        public void RegisterLocalPlayer(Players player)
        {
            _localPlayer = player;
        }
        #endregion
    }
}
