using Abraxas.Zones.Factories;
using Abraxas.Zones.Fields.Controllers;
using Abraxas.Zones.Fields.Managers;
using Abraxas.Zones.Fields.Models;
using Abraxas.Zones.Fields.Views;
using Abraxas.Zones.Models;
using Zenject;

namespace Abraxas.Fields.Installers
{
    class FieldInstaller : Installer<FieldInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<FieldManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<FieldView>().AsTransient();
            Container.BindInterfacesAndSelfTo<FieldController>().AsTransient();
            Container.BindInterfacesAndSelfTo<FieldModel>().AsTransient();

            Container.Bind(typeof(ZoneFactory<IFieldView, FieldController, FieldModel>)).ToSelf().AsTransient();

            Container.Bind<IZoneModel>().To<FieldModel>().WhenInjectedInto<FieldController>();
        }
    }
}
