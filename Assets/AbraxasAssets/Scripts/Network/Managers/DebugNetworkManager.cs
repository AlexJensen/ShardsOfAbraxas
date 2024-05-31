using Unity.Netcode;

namespace Abraxas.Network.Managers
{
    /// <summary>
    /// DebugNetworkManager is a network manager that starts a host if in debug mode.
    /// </summary>
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