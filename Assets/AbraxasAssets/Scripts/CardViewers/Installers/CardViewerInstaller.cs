using Abraxas.CardViewers.Managers;
using Zenject;

namespace Abraxas.CardViewers.Installers
{
    /// <summary>
    /// CardViewerInstaller is a Zenject installer for the card viewer system.
    /// </summary>
    class CardViewerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CardViewerManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}
