using Abraxas.StackBlocks.Views;
using Abraxas.StatBlocks.Controllers;
using Abraxas.StatBlocks.Data;
using Abraxas.StatBlocks.Models;
using Abraxas.StatBlocks.Views;
using Zenject;

namespace Abraxas.StatBlocks.Factories
{

    /// <summary>
    /// StatBlockFactory is a factory for creating stat blocks.
    /// </summary>

    class StatBlockFactory : IFactory<StatBlockData, IStatBlockView, IStatBlockController>
    {
        #region Dependencies
        readonly DiContainer _container;

        public StatBlockFactory(DiContainer container)
        {
            _container = container;
        }
        #endregion

        #region Methods
        public IStatBlockController Create(StatBlockData data, IStatBlockView statBlockView)
        {
            var statBlockModel = _container.Instantiate<StatBlockModel>();
            var statBlockController = _container.Instantiate<StatBlockController>();

            statBlockModel.Initialize(data);
            statBlockController.Initialize(statBlockModel);
            ((StatBlockView)statBlockView).Initialize(statBlockModel, statBlockController);

            return statBlockController;
        }
        #endregion
    }
}