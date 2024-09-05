
using Abraxas.Games.Managers;
using Zenject;

namespace Abraxas.Games.Installers
{
    class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}
