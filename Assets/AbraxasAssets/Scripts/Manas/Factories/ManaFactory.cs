using Abraxas.Manas.Controllers;
using Abraxas.Manas.Models;
using Abraxas.Manas.Views;
using Zenject;

namespace Abraxas.Manas.Factories
{
    class ManaFactory : IFactory<IManaView, IManaController>
    {
        readonly DiContainer _container;

        public ManaFactory(DiContainer container)
        {
            _container = container;
        }
        public IManaController Create(IManaView view)
        {
            var controller = _container.Instantiate<ManaController>();
            var model = _container.Instantiate<ManaModel>();

            controller.Initialize(view, model);
            model.Initialize(view, controller);
            return controller;
        }
    }
}
