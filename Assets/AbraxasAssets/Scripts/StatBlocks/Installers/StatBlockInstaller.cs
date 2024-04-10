using Abraxas.Health.Controllers;
using Abraxas.Health.Factories;
using Abraxas.Health.Managers;
using Abraxas.Health.Models;
using Abraxas.Health.Views;
using Abraxas.StackBlocks.Views;
using Abraxas.StatBlocks.Controllers;
using Abraxas.StatBlocks.Data;
using Abraxas.StatBlocks.Factories;
using Abraxas.StatBlocks.Models;
using Abraxas.StatBlocks.Views;
using System.ComponentModel;
using Zenject;

namespace Abraxas.StatBlocks.Installers
{
    class StatBlockInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<StatBlockView>().AsTransient();
            Container.BindInterfacesAndSelfTo<StatBlockController>().AsTransient();
            Container.BindInterfacesAndSelfTo<StatBlockModel>().AsTransient();

            Container.BindFactory<StatBlockData, IStatBlockView, IStatBlockController, StatBlockController.Factory>().FromFactory<StatBlockFactory>();
        }
    }
}
