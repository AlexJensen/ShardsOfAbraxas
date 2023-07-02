using Abraxas.Cells.Factories;
using Abraxas.Cells.Controllers;
using Abraxas.Cells.Models;
using Abraxas.Cells.Views;
using Zenject;

namespace Abraxas.Cells.Installers
{
    class CellInstaller : MonoInstaller
    {
        #region Bindings
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CellView>().AsTransient();
            Container.BindInterfacesAndSelfTo<CellController>().AsTransient();
            Container.BindInterfacesAndSelfTo<CellModel>().AsTransient();

            Container.BindFactory<ICellView, ICellController, CellController.Factory>().FromFactory<CellFactory>();    
        }
        #endregion
    }
}
