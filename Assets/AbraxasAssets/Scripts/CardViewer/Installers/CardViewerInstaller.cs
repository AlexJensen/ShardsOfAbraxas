using Abraxas.CardViewers.Managers;
using Zenject;

namespace Abraxas.CardViewer.Installers
{
    class CardViewerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CardViewerManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}
