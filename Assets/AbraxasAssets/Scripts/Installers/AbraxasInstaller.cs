using Abraxas.Behaviours.Cards;
using Abraxas.Behaviours.CardViewer;
using Abraxas.Behaviours.Data;
using Abraxas.Behaviours.Game;
using Abraxas.Behaviours.Manas;
using Abraxas.Behaviours.Network;
using Abraxas.Behaviours.Zones.Drags;
using Abraxas.Behaviours.Zones.Fields;
using Abraxas.Scripts.States;
using System;
using UnityEngine;
using Zenject;

namespace Abraxas.Core.Installers
{
    public class AbraxasInstaller : MonoInstaller
    {
        [Inject]
        Settings _settings = null;

        public override void InstallBindings()
        {
            InstallManagers();
            InstallGameStates();
            InstallManaTypes();
        }

        private void InstallManagers()
        {
            Container.Bind<GameManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<DragManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<DataManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<CardViewerManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<FieldManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<DebugNetworkManager>().FromComponentInHierarchy().AsSingle();
        }

        private void InstallGameStates()
        {
            Container.Bind<GameStateFactory>().AsSingle();

            Container.BindFactory<GameNotStartedState, GameNotStartedState.Factory>().WhenInjectedInto<GameStateFactory>();
            Container.BindFactory<BeginningState, BeginningState.Factory>().WhenInjectedInto<GameStateFactory>();
            Container.BindFactory<BeforeCombatState, BeforeCombatState.Factory>().WhenInjectedInto<GameStateFactory>();
            Container.BindFactory<CombatState, CombatState.Factory>().WhenInjectedInto<GameStateFactory>();
            Container.BindFactory<AfterCombatState, AfterCombatState.Factory>().WhenInjectedInto<GameStateFactory>();
            Container.BindFactory<EndState, EndState.Factory>().WhenInjectedInto<GameStateFactory>();
        }

        private void InstallManaTypes()
        {
            Container.BindFactory<ManaType, ManaType.Factory>()
                .FromComponentInNewPrefab(_settings.ManaTypePrefab)
                .WithGameObjectName("ManaType");
        }

        [Serializable]
        public class Settings
        {
            public GameObject ManaTypePrefab;
        }
    }
}
