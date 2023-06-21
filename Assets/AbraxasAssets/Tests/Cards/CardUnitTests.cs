using Abraxas.Cards.Controllers;
using Abraxas.Cards.Data;
using Abraxas.Cards.Factories;
using Abraxas.Cards.Installers;
using Abraxas.Cards.Managers;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.CardViewers;
using Abraxas.Cell.Factories;
using Abraxas.Cells.Controllers;
using Abraxas.Cells.Views;
using Abraxas.Events.Managers;
using Abraxas.Games.Managers;
using Abraxas.Health.Controllers;
using Abraxas.Health.Factories;
using Abraxas.Health.Managers;
using Abraxas.Health.Models;
using Abraxas.Health.Views;
using Abraxas.Manas;
using Abraxas.Players.Installers;
using Abraxas.Players.Managers;
using Abraxas.Stones.Installers;
using Abraxas.Zones.Decks.Managers;
using Abraxas.Zones.Factories;
using Abraxas.Zones.Fields.Controllers;
using Abraxas.Zones.Fields.Managers;
using Abraxas.Zones.Fields.Models;
using Abraxas.Zones.Fields.Views;
using Abraxas.Zones.Hands.Managers;
using Abraxas.Zones.Managers;
using Abraxas.Zones.Overlays.Managers;
using Moq;
using NUnit.Framework;
using System.Collections;
using UnityEditor.SceneManagement;
using Zenject;
using Player = Abraxas.Players.Players;



namespace Abraxas.Tests
{
    class TestInstaller : Installer<TestInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<IGameManager>().FromMock();
            Container.Bind<IPlayerManager>().FromMock();
            Container.Bind<IHandManager>().FromMock();
            Container.Bind<IManaManager>().FromMock();
            Container.Bind<ICardManager>().FromMock();
            Container.Bind<ICardViewerManager>().FromMock();
            Container.Bind<IZoneManager>().FromMock();
            Container.Bind<IDeckManager>().FromMock();
            Container.Bind<IEventManager>().FromMock();
            Container.BindInterfacesAndSelfTo<HealthManager>().FromNewComponentOnNewGameObject().AsSingle();
            Container.Bind<IFieldManager>().FromMock();
            Container.Bind<IOverlayManager>().FromMock();

            Container.BindInterfacesAndSelfTo<CardView>().FromNewComponentOnNewGameObject().AsTransient();
            Container.BindInterfacesAndSelfTo<CardController>().AsTransient();
            Container.BindInterfacesAndSelfTo<CardModel>().AsTransient();

            Container.BindInterfacesAndSelfTo<PlayerHealthView>().FromNewComponentOnNewGameObject().AsTransient();
            Container.BindInterfacesAndSelfTo<PlayerHealthController>().AsTransient();
            Container.BindInterfacesAndSelfTo<PlayerHealthModel>().AsTransient();

            Container.Bind(typeof(ZoneFactory<IFieldView, IFieldController, IFieldModel>)).ToSelf().AsTransient();
            
            Container.BindFactory<CardData, Player, ICardController, CardController.Factory>().FromFactory<CardFactory>();
            Container.BindFactory<IPlayerHealthView, IPlayerHealthController, PlayerHealthController.Factory>().FromFactory<PlayerHealthFactory>();
            Container.BindFactory<ICellView, ICellController, CellController.Factory>().FromFactory<CellFactory>();
        }
    }

    [TestFixture]
    public class CardUnitTests : ZenjectUnitTestFixture
    {
        [SetUp]
        public void LoadTestScene()
        {
            EditorSceneManager.OpenScene("Assets/AbraxasAssets/Scenes/Unit Tests.unity", OpenSceneMode.Single);
        }
        [SetUp]
        public void BindInterfaces()
        {
            CardSettingsInstaller.InstallFromResource("Settings/CardSettings", Container);
            StoneSettingsInstaller.InstallFromResource("Settings/StoneSettings", Container);
            PlayerSettingsInstaller.InstallFromResource("Settings/PlayerSettings", Container);
            TestInstaller.Install(Container);
        }

        [Test]
        public void WillCardViewResolve()
        {
            // Act
            ICardView obj = Container.Resolve<ICardView>();

            // Assert
            Assert.NotNull(obj);
        }

        [Test]
        public void WillCardControllerResolve()
        {
            // Act
            ICardController obj = Container.Resolve<ICardController>();

            // Assert
            Assert.NotNull(obj);
        }

        [Test]
        public void WillCardModelResolve()
        {
            // Act
            ICardModel obj = Container.Resolve<ICardModel>();

            // Assert
            Assert.NotNull(obj);
        }

        [Test]
        public void Create_CardWithDependencies_Success()
        {
            // Arrange
            CardData cardData = new();
            var player = Player.Player1;

            var factory = Container.Resolve<CardController.Factory>();

            // Act
            var cardController = factory.Create(cardData, player);

            // Assert
            Assert.IsNotNull(cardController);
            Assert.IsNotNull(cardController.Model);
            Assert.IsNotNull(cardController.View);
        }

        [Test]
        public void CardController_PassHomeRow_ModifiesPlayerHealth()
        {
            // Arrange
            CardData cardData = new()
            {
                Owner = Player.Player1,
                StatBlock = new()
                {
                    Stats = new(1, 0, 0)
                }
            };

            var healthManager = Container.Resolve<IHealthManager>();
            var healthFactory = Container.Resolve<PlayerHealthController.Factory>();
            var playerHealthViewMock = new Mock<IPlayerHealthView>();
            var playerHealthController = healthFactory.Create(playerHealthViewMock.Object);
            playerHealthController.MaxHP = 1;
            playerHealthController.HP = 1;
            playerHealthController.Player = Player.Player2;
            healthManager.AddPlayerHealth(playerHealthController);

            var cardFactory = Container.Resolve<CardController.Factory>();
            var cardController = cardFactory.Create(cardData, Player.Player1);
            int expectedHealth = playerHealthController.HP - cardData.StatBlock[StatBlocks.StatValues.ATK];

            // Act
            IEnumerator enumerator = cardController.PassHomeRow();
            while (enumerator.MoveNext()) { }

            // Assert
            int player2Health = playerHealthController.HP;
            Assert.AreEqual(expectedHealth, player2Health);
        }

        [Test]
        public void CardController_Fight_DamagesOpponent()
        {
            // Arrange
            CardData cardData1 = new()
            {
                Owner = Player.Player1,
                StatBlock = new()
                {
                    Stats = new(1, 0, 0)
                }
            };

            CardData cardData2 = new()
            {
                Owner = Player.Player2,
                StatBlock = new()
                {
                    Stats = new(1, 0, 0)
                }
            };

            var cardFactory = Container.Resolve<CardController.Factory>();
            var cardController = cardFactory.Create(cardData1, Player.Player1);
            var opponentController = cardFactory.Create(cardData2, Player.Player2);

            // Act
            IEnumerator enumerator = cardController.Fight(opponentController);
            while (enumerator.MoveNext()) { }

            // Assert
            Assert.AreEqual(-1, cardController.StatBlock[StatBlocks.StatValues.DEF]);
            Assert.AreEqual(-1, opponentController.StatBlock[StatBlocks.StatValues.DEF]);
        }

        [Test]
        public void CardController_Fight_DoesNotDamageAllies()
        {
            // Arrange
            CardData cardData1 = new()
            {
                Owner = Player.Player1,
                StatBlock = new()
                {
                    Stats = new(1, 0, 0)
                }
            };

            CardData cardData2 = new()
            {
                Owner = Player.Player1,
                StatBlock = new()
                {
                    Stats = new(1, 0, 0)
                }
            };

            var cardFactory = Container.Resolve<CardController.Factory>();
            var cardController = cardFactory.Create(cardData1, Player.Player1);
            var opponentController = cardFactory.Create(cardData2, Player.Player2);

            // Act
            IEnumerator enumerator = cardController.Fight(opponentController);
            while (enumerator.MoveNext()) { }

            // Assert
            Assert.AreEqual(0, cardController.StatBlock[StatBlocks.StatValues.DEF]);
            Assert.AreEqual(0, opponentController.StatBlock[StatBlocks.StatValues.DEF]);
        }
    }
}
