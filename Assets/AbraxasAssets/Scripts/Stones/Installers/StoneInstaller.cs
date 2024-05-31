
using Abraxas.Events;
using Abraxas.Stones.Controllers;
using Abraxas.Stones.Controllers.StoneTypes.Conditions;
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
            Container.BindFactory<StoneDataSO, IStoneController, StoneController.Factory>().FromFactory<StoneFactory>();


            Container.Bind<Condition<GameStateEnteredEvent>>().To<StartOfStateCondition>().AsSingle();
            Container.Bind<Condition<object>>().To<IsActivePlayerCondition>().AsSingle();

            Container.BindInterfacesAndSelfTo<Effect_DrawCardFromDeck>().AsTransient();
        }
    }
    #endregion
}
