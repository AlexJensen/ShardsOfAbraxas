using Abraxas.Zones.Factories;
using Abraxas.Zones.Graveyard.Controllers;
using Abraxas.Zones.Graveyards.Managers;
using Abraxas.Zones.Graveyards.Models;
using Abraxas.Zones.Graveyards.Views;
using Abraxas.Zones.Models;
using Zenject;

namespace Abraxas.Graveyards.Installers
{
    class GraveyardInstaller : Installer<GraveyardInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GraveyardManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<GraveyardView>().AsTransient();
            Container.BindInterfacesAndSelfTo<GraveyardController>().AsTransient();
            Container.BindInterfacesAndSelfTo<GraveyardModel>().AsTransient();

            Container.Bind(typeof(ZoneFactory<IGraveyardView, GraveyardController, GraveyardModel>)).ToSelf().AsTransient();

            Container.Bind<IZoneModel>().To<GraveyardModel>().WhenInjectedInto<GraveyardController>();
        }
    }
}
