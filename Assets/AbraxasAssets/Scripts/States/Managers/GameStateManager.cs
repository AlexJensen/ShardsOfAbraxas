using System.Collections;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Abraxas.GameStates.Managers
{

    public class GameStateManager : NetworkBehaviour, IGameStateManager
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
        private int clientAcknowledgments = 0;
        private bool isWaitingForClientAcknowledgments = false;
        private bool isWaitingForServer = true;

        public GameState State { get => _state; }
        #endregion

        #region Methods
        public IEnumerator BeginNextGameState()
        {
            if (!IsServer) yield break;
            Debug.Log($"BeginNextGameState;");
            yield return SwitchGameStateTo(State.NextState());
        }
        public IEnumerator RequestNextGameState()
        {
            Debug.Log($"RequestNextGameState;");
            if (!IsClient) yield break;
            SetGameStateServerRpc(State.NextState());
        }
        public IEnumerator InitializeState(GameStates state)
        {
            Debug.Log($"InitializeState; {state}");
            _state = _stateFactory.CreateState(state);
            yield return null;
            yield return State?.OnEnterState();
            yield return null;
        }
        private IEnumerator SwitchGameStateTo(GameStates state)
        {
            if (!IsClient)
            {
                Debug.Log($"Server: State?.OnExitState(); {_state.CurrentState}");
                isWaitingForClientAcknowledgments = true;
                clientAcknowledgments = 0;
                SetGameStateClientRpc(state);
                while (clientAcknowledgments < NetworkManager.Singleton.ConnectedClients.Count) yield return null;
                isWaitingForClientAcknowledgments = false;
                yield return State?.OnExitState();
                _state = _stateFactory.CreateState(state);
                Debug.Log($"Server: State?.OnEnterState(); {_state.CurrentState}");
                yield return State?.OnEnterState();
                clientAcknowledgments = 0;
                AdvanceGameStateClientRpc();
                
            }
            else
            {
                Debug.Log($"Client: State?.OnExitState(); {_state.CurrentState}");
                yield return State?.OnExitState();
                _state = _stateFactory.CreateState(state);
                Debug.Log($"Client: State?.OnEnterState();{_state.CurrentState}");
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
        [ServerRpc(RequireOwnership = false)]
        private void AcknowledgeServerRpc()
        {
            if (!isWaitingForClientAcknowledgments) return;
            Debug.Log($"AcknowledgeServerRpc {clientAcknowledgments} + 1");
            clientAcknowledgments++;
        }
        #endregion
        #region Client Side
        [ClientRpc]
        private void SetGameStateClientRpc(GameStates state)
        {
            Debug.Log($"SetGameStateClientRpc {state}");
            StartCoroutine(SwitchGameStateTo(state));
        }
        [ClientRpc]
        private void AdvanceGameStateClientRpc()
        {
            Debug.Log($"AdvanceGameStateClientRpc");
            isWaitingForServer = false;
        }

        #endregion
        #endregion
    }
}
