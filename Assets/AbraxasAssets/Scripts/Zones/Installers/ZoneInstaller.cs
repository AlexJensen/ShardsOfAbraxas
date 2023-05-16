using Abraxas.Zones.Decks.Controllers;
using Abraxas.Zones.Decks.Models;
using Abraxas.Zones.Decks.Views;
using Abraxas.Zones.Factories;
using Abraxas.Zones.Fields.Controllers;
using Abraxas.Zones.Fields.Models;
using Abraxas.Zones.Fields.Views;
using Abraxas.Zones.Graveyard.Controllers;
using Abraxas.Zones.Graveyards.Models;
using Abraxas.Zones.Graveyards.Views;
using Abraxas.Zones.Hands.Controllers;
using Abraxas.Zones.Hands.Models;
using Abraxas.Zones.Hands.Views;
using Abraxas.Zones.Models;
using Zenject;

namespace Abraxas
{
    public class ZoneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<FieldView>().AsTransient();
            Container.BindInterfacesAndSelfTo<FieldController>().AsTransient();
            Container.BindInterfacesAndSelfTo<FieldModel>().AsTransient();
            Container.BindInterfacesAndSelfTo<HandView>().AsTransient();
            Container.BindInterfacesAndSelfTo<HandController>().AsTransient();
            Container.BindInterfacesAndSelfTo<HandModel>().AsTransient();
            Container.BindInterfacesAndSelfTo<DeckView>().AsTransient();
            Container.BindInterfacesAndSelfTo<DeckController>().AsTransient();
            Container.BindInterfacesAndSelfTo<DeckModel>().AsTransient();
            Container.BindInterfacesAndSelfTo<GraveyardView>().AsTransient();
            Container.BindInterfacesAndSelfTo<GraveyardController>().AsTransient();
            Container.BindInterfacesAndSelfTo<GraveyardModel>().AsTransient();

            Container.Bind(typeof(ZoneFactory<IFieldView, FieldController, FieldModel>)).ToSelf().AsTransient();
            Container.Bind(typeof(ZoneFactory<IHandView, HandController, HandModel>)).ToSelf().AsTransient();
            Container.Bind(typeof(ZoneFactory<IDeckView, DeckController, DeckModel>)).ToSelf().AsTransient();
            Container.Bind(typeof(ZoneFactory<IGraveyardView, GraveyardController, GraveyardModel>)).ToSelf().AsTransient();

            Container.Bind<IZoneModel>().To<FieldModel>().WhenInjectedInto<FieldController>();
            Container.Bind<IZoneModel>().To<HandModel>().WhenInjectedInto<HandController>();
            Container.Bind<IZoneModel>().To<DeckModel>().WhenInjectedInto<DeckController>();
            Container.Bind<IZoneModel>().To<GraveyardModel>().WhenInjectedInto<GraveyardController>();
        }
    }
}
