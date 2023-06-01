using Abraxas.Health.Managers;
using Zenject;

namespace Abraxas.Health.Installers
{
    class HealthInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<HealthManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}
