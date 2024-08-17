using Abraxas.Events;
using Abraxas.Events.Managers;
using Abraxas.Network.Managers;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Abraxas.Players.Managers
{

    /// <summary>
    /// PlayerManager is a class for managing players.
    /// </summary>

    public class PlayerManager : NetworkedManager, IPlayerManager
    {

        #region Dependencies
        IEventManager _eventManager;
        [Inject]
        public void Construct(IEventManager eventManager)
        {
            _eventManager = eventManager;
        }

        #endregion

        #region Fields
        readonly NetworkVariable<Players> _activePlayer = new(Players.Player1);
        Players _localPlayer = Players.Player1;

        #endregion

        #region Properties
        public Players ActivePlayer => _activePlayer.Value;
        public Players LocalPlayer => _localPlayer;
        #endregion

        #region Methods
        public override void OnNetworkSpawn()
        {
            _activePlayer.OnValueChanged += OnActivePlayerChanged;
            if (IsServer)
            {
                StartCoroutine(_eventManager.RaiseEvent(new Event_LocalPlayerChanged(LocalPlayer)));
            }
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
                
            }
            StartCoroutine(ChangeActivePlayer(current));
        }

        private IEnumerator ChangeActivePlayer(Players player)
        {
            yield return _eventManager.RaiseEvent(new Event_ActivePlayerChanged(player));

            if (IsClient)
            {
                AcknowledgeServerRpc();
            }
        }

        public IEnumerator SetActivePlayer(Players player)
        {
            if (!IsServer) yield break;
            Debug.Log($"SetActivePlayer: {player}");
            if (IsHost) _localPlayer = player;

            _activePlayer.Value = player;
            
            yield return WaitForClients();
        }

        public IEnumerator RegisterLocalPlayer(Players player)
        {
            _localPlayer = player;
            yield return _eventManager.RaiseEvent(new Event_LocalPlayerChanged(player));
        }
        #endregion
    }
}
