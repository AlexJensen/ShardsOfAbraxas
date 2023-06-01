using Abraxas.Players.Managers;
using Zenject;

namespace Abraxas.Players.Installers
{
    class PlayerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PlayerManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}
