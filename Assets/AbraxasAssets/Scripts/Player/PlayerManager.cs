using System.Collections;
using Unity.Netcode;
using UnityEngine;
using Zenject;

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
        public IEnumerator ToggleActivePlayer()
        {
            if (!IsServer) yield break;
            _activePlayer.Value = _activePlayer.Value == Players.Player1 ? Players.Player2 : Players.Player1;
        }

        public void RegisterLocalPlayer(Players player)
        {
            _localPlayer = player;
        }
        #endregion
    }
}
