using Abraxas.Health.Controllers;
using Abraxas.Health.Models;
using Abraxas.Health.Views;
using Zenject;

namespace Abraxas.Health.Factories
{
    /// <summary>
    /// PlayerHealthFactory is a Zenject factory for creating player health controllers.
    /// </summary>
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

            
            view.Initialize(model);
            controller.Initialize(view, model);
            model.Initialize();

            return controller;
        }
    }
}
