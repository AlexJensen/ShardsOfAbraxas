using Abraxas.Stones.Controllers;
using Abraxas.Stones.Data;
using Abraxas.Stones.Factories;
using Abraxas.Stones.Models;
using Abraxas.Stones.Views;
using Zenject;

namespace Abraxas.Cards.Installers
{
    public class StoneInstaller : MonoInstaller<StoneInstaller>
    {
        #region Bindings
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<StoneView>().AsTransient();
            Container.BindInterfacesAndSelfTo<StoneController>().AsTransient();
            Container.BindInterfacesAndSelfTo<StoneModel>().AsTransient();
            Container.BindFactory<StoneDataSO, IStoneController, StoneController.Factory>().FromFactory<StoneFactory>();
        }
    }
        #endregion
}
