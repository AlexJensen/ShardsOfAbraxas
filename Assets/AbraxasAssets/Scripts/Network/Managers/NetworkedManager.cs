using Abraxas.Core;
using System.Collections;
using Unity.Netcode;

namespace Abraxas.Network.Managers
{

    /// <summary>
    /// NetworkedManager is a base class for all networked managers in the game.
    /// </summary>
    public class NetworkedManager : NetworkBehaviour, IManager
    {
        #region Fields
        protected int clientAcknowledgments = 0;
        protected bool isWaitingForClientAcknowledgments = false;
        #endregion

        #region Methods
        protected IEnumerator WaitForClients()
        {
            if (IsHost)
            {
                yield break;
            }
            isWaitingForClientAcknowledgments = true;
            clientAcknowledgments = 0;
            while (clientAcknowledgments < NetworkManager.Singleton.ConnectedClients.Count)
            {
                yield return null;
            }
            isWaitingForClientAcknowledgments = false;
        }

        [ServerRpc(RequireOwnership = false)]
        protected void AcknowledgeServerRpc()
        {
            if (!isWaitingForClientAcknowledgments) return;
            clientAcknowledgments++;
        }
        #endregion
    }
}
