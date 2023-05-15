using Unity.Netcode;

namespace Abraxas.Network.Managers
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