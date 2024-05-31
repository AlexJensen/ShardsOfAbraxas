using Abraxas.Health.Controllers;
using Abraxas.Health.Factories;
using Abraxas.Health.Managers;
using Abraxas.Health.Models;
using Abraxas.Health.Views;
using Zenject;

namespace Abraxas.Health.Installers
{
    /// <summary>
    /// PlayerHealthInstaller is a Unity-Specific Zenject installer for the player health system.
    /// </summary>
    class PlayerHealthInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PlayerHealthManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerHealthView>().AsTransient();
            Container.BindInterfacesAndSelfTo<PlayerHealthController>().AsTransient();
            Container.BindInterfacesAndSelfTo<PlayerHealthModel>().AsTransient();

            Container.BindFactory<IPlayerHealthView, IPlayerHealthController, PlayerHealthController.Factory>().FromFactory<PlayerHealthFactory>();
        }
    }
}
