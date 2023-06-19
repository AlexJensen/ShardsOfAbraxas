using Abraxas.Health.Controllers;
using Abraxas.Health.Models;
using Abraxas.Health.Views;
using Zenject;

namespace Abraxas.Health.Factories
{
    class PlayerHealthFactory : IFactory<IPlayerHealthView, IPlayerHealthController>
    {
        readonly DiContainer _container;

        public PlayerHealthFactory(DiContainer container)
        {
            _container = container;
        }

        public IPlayerHealthController Create(IPlayerHealthView view)
        {
            var controller = _container.Instantiate<PlayerHealthController>();
            var model = _container.Instantiate<PlayerHealthModel>();

            controller.Initialize(view, model);
            return controller;
        }
    }
}
