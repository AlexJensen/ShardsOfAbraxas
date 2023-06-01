using Abraxas.GameStates;
using Abraxas.GameStates.Factories;
using Abraxas.GameStates.Managers;
using Zenject;

namespace Abraxas.States.Installers
{
    class GameStateInstaller : MonoInstaller 
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameStateManager>().FromComponentInHierarchy().AsSingle();

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
