using Abraxas.Network.Managers;
using Zenject;

namespace Abraxas.Network.Installers
{
    class NetworkInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<DebugNetworkManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}
