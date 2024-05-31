using Abraxas.Network.Managers;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Abraxas.Players.Managers
{

    /// <summary>
    /// PlayerManager is a class for managing players.
    /// </summary>

    public class PlayerManager : NetworkedManager, IPlayerManager
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
        public override void OnNetworkSpawn()
        {
            _activePlayer.OnValueChanged += OnActivePlayerChanged;
        }

        public override void OnNetworkDespawn()
        {
            _activePlayer.OnValueChanged -= OnActivePlayerChanged;
        }

        private void OnActivePlayerChanged(Players previous, Players current)
        {
            if (IsClient)
            {
                Debug.Log($"OnActivePlayerChanged: {previous} -> {current}");
                AcknowledgeServerRpc();
            }
        }

        public IEnumerator SetActivePlayer(Players player)
        {
            Debug.Log($"SetActivePlayer: {player}");
            if (IsHost) _localPlayer = player;

            _activePlayer.Value = player;
            yield return WaitForClients();
        }

        public void RegisterLocalPlayer(Players player)
        {
            _localPlayer = player;
        }
        #endregion
    }
}
