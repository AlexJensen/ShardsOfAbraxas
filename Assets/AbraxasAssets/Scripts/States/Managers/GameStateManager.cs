using Abraxas.Network.Managers;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Abraxas.GameStates.Managers
{

    /// <summary>
    /// GameStateManager is a NetworkedManager that manages the current game state.
    /// </summary>

    public class GameStateManager : NetworkedManager, IGameStateManager
    {
        #region Dependencies
        IGameStateFactory _stateFactory;

        [Inject]
        public void Construct(IGameStateFactory stateFactory)
        {
            _stateFactory = stateFactory;
        }
        #endregion

        #region Fields
        GameState _state = null;
        private bool isWaitingForServer = true;

        public GameState State { get => _state; }
        #endregion

        #region Methods
        public IEnumerator BeginNextGameState()
        {
            if (!IsServer) yield break;
            yield return SwitchGameStateTo(State.NextState());
        }
        public void RequestNextGameState()
        {
            if (!IsClient) return;
            SetGameStateServerRpc(State.NextState());
        }
        public IEnumerator InitializeState(GameStates state)
        {
            _state = _stateFactory.CreateState(state);
            yield return null;
            yield return State?.OnEnterState();
            yield return null;
        }
        private IEnumerator SwitchGameStateTo(GameStates state)
        {
            if (!IsClient)
            {

                SetGameStateClientRpc(state);
                yield return WaitForClients();

                yield return State?.OnExitState();
                _state = _stateFactory.CreateState(state);
                yield return State?.OnEnterState();

                AdvanceGameStateClientRpc();
            }
            else
            {
                yield return State?.OnExitState();
                _state = _stateFactory.CreateState(state);
                yield return State?.OnEnterState();

                isWaitingForServer = true;
                AcknowledgeServerRpc();
                while (isWaitingForServer) yield return null;
            }
        }
        #endregion

        #region Server Methods
        #region Server Side
        [ServerRpc(RequireOwnership = false)]
        private void SetGameStateServerRpc(GameStates state)
        {
            Debug.Log($"SetGameStateServerRpc {state}");
            StartCoroutine(SwitchGameStateTo(state));
        }
        #endregion
        #region Client Side
        [ClientRpc]
        private void SetGameStateClientRpc(GameStates state)
        {
            StartCoroutine(SwitchGameStateTo(state));
        }
        [ClientRpc]
        private void AdvanceGameStateClientRpc()
        {
            isWaitingForServer = false;
        }

        #endregion
        #endregion
    }
}
