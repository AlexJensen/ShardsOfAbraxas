using Abraxas.Health.Controllers;
using Abraxas.Health.Factories;
using Abraxas.Health.Managers;
using Abraxas.Health.Models;
using Abraxas.Health.Views;
using Zenject;

namespace Abraxas.Health.Installers
{
    class PlayerHealthInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<HealthManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerHealthView>().AsTransient();
            Container.BindInterfacesAndSelfTo<PlayerHealthController>().AsTransient();
            Container.BindInterfacesAndSelfTo<PlayerHealthModel>().AsTransient();

            Container.BindFactory<IPlayerHealthView, IPlayerHealthController, PlayerHealthController.Factory>().FromFactory<PlayerHealthFactory>();
        }
    }
}
