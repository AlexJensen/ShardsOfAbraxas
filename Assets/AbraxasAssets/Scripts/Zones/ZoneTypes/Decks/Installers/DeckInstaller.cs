using Abraxas.Zones.Decks.Controllers;
using Abraxas.Zones.Decks.Managers;
using Abraxas.Zones.Decks.Models;
using Abraxas.Zones.Decks.Views;
using Abraxas.Zones.Factories;
using Abraxas.Zones.Models;
using Zenject;

namespace Abraxas.Decks.Installers
{
    class DeckInstaller : Installer<DeckInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<DeckManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<DeckView>().AsTransient();
            Container.BindInterfacesAndSelfTo<DeckController>().AsTransient();
            Container.BindInterfacesAndSelfTo<DeckModel>().AsTransient();

            Container.Bind(typeof(ZoneFactory<IDeckView, DeckController, DeckModel>)).ToSelf().AsTransient();

            Container.Bind<IZoneModel>().To<DeckModel>().WhenInjectedInto<DeckController>();
        }
    }
}
