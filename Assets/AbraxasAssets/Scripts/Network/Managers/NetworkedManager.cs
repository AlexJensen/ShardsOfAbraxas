using System.Collections;
using Unity.Netcode;

namespace Abraxas.Network.Managers
{
	public class NetworkedManager : NetworkBehaviour
	{
		#region Fields
		protected int clientAcknowledgments = 0;
		protected bool isWaitingForClientAcknowledgments = false;
		#endregion

		#region Methods
		protected IEnumerator WaitForClients()
		{
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
