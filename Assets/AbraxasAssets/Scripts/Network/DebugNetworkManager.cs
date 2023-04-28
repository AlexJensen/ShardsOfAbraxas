using Abraxas.Core;
using Unity.Netcode;

namespace Abraxas.Network
{
    public class DebugNetworkManager : NetworkBehaviour
    {
        public bool isDebugMode = false;

        private void Start()
        {
            if (isDebugMode)
            {
                NetworkManager.StartHost();
            }
        }
    }
}