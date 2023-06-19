using Abraxas.Zones.Factories;
using Abraxas.Zones.Hands.Controllers;
using Abraxas.Zones.Hands.Managers;
using Abraxas.Zones.Hands.Models;
using Abraxas.Zones.Hands.Views;
using Abraxas.Zones.Models;
using Zenject;

namespace Abraxas.Hands.Installers
{
    public class HandInstaller : Installer<HandInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<HandManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<HandView>().AsTransient();
            Container.BindInterfacesAndSelfTo<HandController>().AsTransient();
            Container.BindInterfacesAndSelfTo<HandModel>().AsTransient();

            Container.Bind(typeof(ZoneFactory<IHandView, HandController, HandModel>)).ToSelf().AsTransient();

            Container.Bind<IZoneModel>().To<HandModel>().WhenInjectedInto<HandController>();
        }
    }
}
