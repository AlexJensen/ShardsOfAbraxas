using Abraxas.Cells.Controllers;
using Zenject;

namespace Abraxas.Cells.Installers
{
    class CellInstaller : MonoInstaller
    {
        #region Bindings
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CellController>().AsTransient();
        }
        #endregion
    }
}
