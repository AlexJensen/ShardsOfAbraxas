using Abraxas.Zones.Overlays.Managers;
using Zenject;

namespace Abraxas.Overlays.Installers
{
    /// <summary>
    /// OverlayInstaller is a MonoInstaller that installs OverlayManager into the Zenject container.
    /// </summary>
    class OverlayInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<OverlayManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}
