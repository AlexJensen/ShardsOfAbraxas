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

        public GameState State { get => _state; }
        #endregion

        #region Methods
        public IEnumerator SwitchGameStateTo(GameStates state)
        {
            yield return new WaitForSeconds(0.1f);
            yield return State?.OnExitState();
            yield return new WaitForSeconds(0.1f);
            _state = _stateFactory.CreateState(state);
            yield return new WaitForSeconds(0.1f);
            yield return State?.OnEnterState();
            yield return new WaitForSeconds(0.1f);
        }

        public IEnumerator BeginNextGameState()
        {
            if (!IsServer) yield break;
            AdvanceGameStateClientRpc();
            if (!IsHost) yield return SwitchGameStateTo(State.NextState());
        }

        public IEnumerator RequestNextGameState()
        {
            if (!IsClient) yield break;
            AdvanceGameStateServerRpc();
        }

        private IEnumerator WaitForGameStateInitialized()
        {
            while (State == null)
            {
                yield return null;
            }
            yield return SwitchGameStateTo(State.NextState());
        }
        #endregion

        #region Server Methods
        [ServerRpc(RequireOwnership = false)]
        private void AdvanceGameStateServerRpc()
        {
            if (!IsServer) return;
            if (!IsHost) StartCoroutine(SwitchGameStateTo(State.NextState()));
            AdvanceGameStateClientRpc();
        }

        [ClientRpc]
        private void AdvanceGameStateClientRpc()
        {
            if (!IsClient) return;
            StartCoroutine(WaitForGameStateInitialized());
        }
        #endregion
    }
}
