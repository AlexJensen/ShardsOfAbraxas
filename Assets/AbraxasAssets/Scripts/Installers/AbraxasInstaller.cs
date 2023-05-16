using Abraxas.CardViewers.Managers;
using Abraxas.Events.Managers;
using Abraxas.Game.Managers;
using Abraxas.GameStates;
using Abraxas.GameStates.Factories;
using Abraxas.GameStates.Managers;
using Abraxas.Health.Managers;
using Abraxas.Manas;
using Abraxas.Manas.Managers;
using Abraxas.Network.Managers;
using Abraxas.Players.Managers;
using Abraxas.UI.Managers;
using Abraxas.Zones.Decks.Managers;
using Abraxas.Zones.Fields.Managers;
using Abraxas.Zones.Graveyards.Managers;
using Abraxas.Zones.Hands.Managers;
using Abraxas.Zones.Managers;
using Abraxas.Zones.Overlays.Managers;
using Zenject;

namespace Abraxas.Core.Installers
{
    public class AbraxasInstaller : MonoInstaller
    {
        [Inject] private readonly Mana.Settings _manaSettings;
        public override void InstallBindings()
        {
            InstallManagers();
            InstallGameStateFactories();
            InstallManaFactories();
        }

        private void InstallManaFactories()
        {
            Container.BindFactory<ManaType, ManaType.Factory>().FromComponentInNewPrefab(_manaSettings.ManaTypePrefab);
        }

        private void InstallManagers()
        {
            Container.BindInterfacesAndSelfTo<GameManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<ZoneManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<DebugNetworkManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<CardViewerManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<ManaManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<HandManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<DeckManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<GraveyardManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<FieldManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<EventManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<HealthManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<OverlayManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<MenuManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<GameStateManager>().FromComponentInHierarchy().AsSingle();
        }

        private void InstallGameStateFactories()
        {
            Container.BindInterfacesAndSelfTo<GameStateFactory>().AsSingle();

            Container.BindFactory<GameNotStartedState, GameNotStartedState.Factory>().WhenInjectedInto<GameStateFactory>();
            Container.BindFactory<BeginningState, BeginningState.Factory>().WhenInjectedInto<GameStateFactory>();
            Container.BindFactory<BeforeCombatState, BeforeCombatState.Factory>().WhenInjectedInto<GameStateFactory>();
            Container.BindFactory<CombatState, CombatState.Factory>().WhenInjectedInto<GameStateFactory>();
            Container.BindFactory<AfterCombatState, AfterCombatState.Factory>().WhenInjectedInto<GameStateFactory>();
            Container.BindFactory<EndState, EndState.Factory>().WhenInjectedInto<GameStateFactory>();
        }
    }
}
