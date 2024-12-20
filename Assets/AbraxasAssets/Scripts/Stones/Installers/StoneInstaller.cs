using Abraxas.Cards.Controllers;
using Abraxas.Stones.Controllers;
using Abraxas.Stones.Data;
using Abraxas.Stones.Factories;
using Abraxas.Stones.Models;
using Zenject;

namespace Abraxas.Cards.Installers
{
    public class StoneInstaller : MonoInstaller<StoneInstaller>
    {
        #region Bindings
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<StoneController>().AsTransient();
            Container.BindInterfacesAndSelfTo<StoneModel>().AsTransient();
            Container.BindFactory<StoneSO, ICardController, IStoneController, StoneController.Factory>().FromFactory<StoneFactory>();
        }
        #endregion
    }


}
