using Abraxas.Cells.Controllers;
using Abraxas.Cells.Models;
using Abraxas.Cells.Views;
using Zenject;

namespace Abraxas.Cells.Factories
{
    class CellFactory : IFactory<ICellView, ICellController>
    {
        readonly DiContainer _container;

        public CellFactory(DiContainer container)
        {
            _container = container;
        }

        public ICellController Create(ICellView view)
        {
            var controller = _container.Instantiate<CellController>();
            var model = _container.Instantiate<CellModel>();

            view.Initialize(controller);
            controller.Initialize(view, model);
            return controller;
        }
    }
}
