
using Abraxas.Events;
using Abraxas.Stones.Conditions;
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
            Container.BindFactory<StoneSO, IStoneController, StoneController.Factory>().FromFactory<StoneFactory>();

            Container.Bind<ConditionSO<Event_GameStateEntered>>().To<Condition_IsCurrentState>().AsSingle();
            Container.Bind<ConditionSO<Event_ActivePlayerChanged>>().To<Condition_IsTargetPlayerActivePlayer>().AsSingle();
            Container.Bind<ConditionSO<Event_CardChangedZones>>().To<Condition_WasTargetCardInZone>().AsSingle();
            Container.Bind<ConditionSO<Event_CardChangedZones>>().To<Condition_IsTargetCardInZone>().AsSingle();

            Container.BindInterfacesAndSelfTo<Effect_TargetPlayerDrawsCardsFromDeck>().AsTransient();
        }
    }
    #endregion
}
