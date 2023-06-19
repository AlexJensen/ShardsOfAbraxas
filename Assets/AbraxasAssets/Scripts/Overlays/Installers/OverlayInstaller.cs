using Abraxas.Zones.Overlays.Managers;
using Zenject;

namespace Abraxas.Overlays.Installers
{
    class OverlayInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<OverlayManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}
