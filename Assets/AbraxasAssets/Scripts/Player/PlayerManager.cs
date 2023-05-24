using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Abraxas.Players.Managers
{
    public class PlayerManager : NetworkBehaviour, IPlayerManager
    {
        #region Fields
        readonly NetworkVariable<Players> _activePlayer = new(Players.Player1);
        Players _localPlayer;

        private int clientAcknowledgments = 0;
        private bool isWaitingForClientAcknowledgments = false;
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
            clientAcknowledgments = 0;
            isWaitingForClientAcknowledgments = true;
            _activePlayer.Value = player;

            while (clientAcknowledgments < NetworkManager.Singleton.ConnectedClients.Count)
            {
                yield return null;
            }
            isWaitingForClientAcknowledgments = false;
        }

        public void RegisterLocalPlayer(Players player)
        {
            _localPlayer = player;
        }

        [ServerRpc(RequireOwnership = false)]
        private void AcknowledgeServerRpc()
        {
            if (!isWaitingForClientAcknowledgments) return;
            clientAcknowledgments++;
        }
        #endregion
    }
}
