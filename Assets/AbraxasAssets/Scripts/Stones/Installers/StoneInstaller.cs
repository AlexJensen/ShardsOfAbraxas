
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

            Container.Bind<Condition<GameStateEnteredEvent>>().To<IsCurrentStateCondition>().AsSingle();
            Container.Bind<Condition<ActivePlayerChangedEvent>>().To<IsActivePlayerCondition>().AsSingle();
            Container.Bind<Condition<CardChangedZonesEvent>>().To<WasThisCardInZonePreviouslyCondition>().AsSingle();
            Container.Bind<Condition<CardChangedZonesEvent>>().To<IsThisCardInZoneCondition>().AsSingle();

            Container.BindInterfacesAndSelfTo<Effect_DrawCardFromDeck>().AsTransient();
        }
    }
    #endregion
}
