using Abraxas.Game;
using Abraxas.Manas;
using Abraxas.GameStates;
using Zenject;
using Abraxas.Zones.Overlays;
using Abraxas.Players;
using Abraxas.CardViewers;
using Abraxas.Zones.Hands;
using Abraxas.Zones.Decks;
using Abraxas.Zones.Graveyards;
using Abraxas.Zones.Fields;
using Abraxas.Events;
using Abraxas.Network;
using Abraxas.UI;

namespace Abraxas.Core.Installers
{
    public class AbraxasInstaller : MonoInstaller
    {
        [Inject] readonly Mana.Settings _manaSettings;
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
            Container.BindInterfacesAndSelfTo<DebugNetworkManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<CardViewerManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<ManaManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<HandManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<DeckManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<GraveyardManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<FieldManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<EventManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<OverlayManager>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<MenuManager>().FromComponentInHierarchy().AsSingle();

            Container.BindInterfacesAndSelfTo<GameStateManager>().FromNew().AsSingle();
        }

        private void InstallGameStateFactories()
        {
            Container.Bind<IGameStateFactory>().To<GameStateFactory>().AsSingle();

            Container.BindFactory<GameNotStartedState, GameNotStartedState.Factory>().WhenInjectedInto<GameStateFactory>();
            Container.BindFactory<BeginningState, BeginningState.Factory>().WhenInjectedInto<GameStateFactory>();
            Container.BindFactory<BeforeCombatState, BeforeCombatState.Factory>().WhenInjectedInto<GameStateFactory>();
            Container.BindFactory<CombatState, CombatState.Factory>().WhenInjectedInto<GameStateFactory>();
            Container.BindFactory<AfterCombatState, AfterCombatState.Factory>().WhenInjectedInto<GameStateFactory>();
            Container.BindFactory<EndState, EndState.Factory>().WhenInjectedInto<GameStateFactory>();
        }
    }
}
