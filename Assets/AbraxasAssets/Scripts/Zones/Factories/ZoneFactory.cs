using Abraxas.Zones.Controllers;
using Abraxas.Zones.Models;
using Abraxas.Zones.Views;
using Zenject;

namespace Abraxas.Zones.Factories
{

    public class ZoneFactory<TView, TController, TModel> : IFactory<TView, TController>
    where TView : IZoneView
    where TController : IZoneController
    where TModel : IZoneModel
    {
        private readonly DiContainer _container;

        public ZoneFactory(DiContainer container)
        {
            _container = container;
        }

        public TController Create(TView view)
        {
            var controller = _container.Instantiate<TController>();
            var model = _container.Instantiate<TModel>();

            controller.Initialize(view, model);
            return controller;
        }
    }
}

