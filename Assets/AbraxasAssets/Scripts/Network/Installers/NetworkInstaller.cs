using Abraxas.Network.Managers;
using Zenject;

namespace Abraxas.Network.Installers
{
    /// <summary>
    /// NetworkInstaller is a MonoInstaller that installs network-related dependencies into the Zenject container.
    /// </summary>
    class NetworkInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<DebugNetworkManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}
