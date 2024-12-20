using Abraxas.VFX.Managers;
using Zenject;

namespace Abraxas.VFX.Installers
{
    internal class VFXInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<VFXManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}
