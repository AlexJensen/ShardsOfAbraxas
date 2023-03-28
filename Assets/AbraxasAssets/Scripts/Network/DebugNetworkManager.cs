using Abraxas.Core;
using Unity.Netcode;

namespace Abraxas.Behaviours.Network
{
    public class DebugNetworkManager : Singleton<DebugNetworkManager>
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