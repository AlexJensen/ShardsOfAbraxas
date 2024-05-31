using Abraxas.Players.Managers;
using Zenject;

namespace Abraxas.Players.Installers
{
    /// <summary>
    /// PlayerInstaller is a MonoInstaller that installs PlayerManager into the Zenject container.
    /// </summary>
    class PlayerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PlayerManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}
