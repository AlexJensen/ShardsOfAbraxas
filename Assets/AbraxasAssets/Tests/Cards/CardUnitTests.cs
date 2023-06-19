using Abraxas.Assets.AbraxasAssets.Scripts.Stones.Installers;
using Abraxas.Cards.Controllers;
using Abraxas.Cards.Data;
using Abraxas.Cards.Factories;
using Abraxas.Cards.Installers;
using Abraxas.Cards.Managers;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.CardViewers;
using Abraxas.Events.Managers;
using Abraxas.Games.Managers;
using Abraxas.Health.Controllers;
using Abraxas.Manas;
using Abraxas.Players.Installers;
using Abraxas.Players.Managers;
using Abraxas.Zones.Decks.Managers;
using Abraxas.Zones.Fields.Managers;
using Abraxas.Zones.Hands.Managers;
using Abraxas.Zones.Managers;
using Abraxas.Zones.Overlays.Managers;
using Moq;
using NUnit.Framework;
using System.Collections;
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
            Container.Bind<IHealthManager>().FromMock();
            Container.Bind<IFieldManager>().FromMock();
            Container.Bind<IOverlayManager>().FromMock();

            Container.BindInterfacesAndSelfTo<CardView>().FromNewComponentOnNewGameObject().AsTransient();
            Container.BindInterfacesAndSelfTo<CardController>().AsTransient();
            Container.BindInterfacesAndSelfTo<CardModel>().AsTransient();

            Container.BindFactory<CardData, Player, ICardController, CardController.Factory>().FromFactory<CardFactory>();
        }
    }

    [TestFixture]
    public class CardUnitTests : ZenjectUnitTestFixture
    {
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
                    Stats = new (1, 0, 0)
                }
            };

            var healthManagerMock = new Mock<IHealthManager>();
            var playerHealthMock = new Mock<IPlayerHealthController>();
            playerHealthMock.Object.HP = 1;

            healthManagerMock.Setup(hm => hm.GetPlayerHealth(It.IsAny<Player>())).Returns(playerHealthMock.Object);

            var zoneManagerMock = new Mock<IZoneManager>();
            var deckManagerMock = new Mock<IDeckManager>();

            var cardFactory = Container.Resolve<CardController.Factory>();
            var cardController = cardFactory.Create(cardData, Player.Player1);
            int expectedHealth = playerHealthMock.Object.HP - cardData.StatBlock[StatBlocks.StatValues.ATK];

            // Act
            IEnumerator enumerator = cardController.PassHomeRow();
            while (enumerator.MoveNext()) { }

            // Assert
            int player2Health = playerHealthMock.Object.HP;

            healthManagerMock.Verify(hm => hm.ModifyPlayerHealth(
                It.IsAny<Player>(), It.Is<int>(val => val == -cardData.StatBlock[StatBlocks.StatValues.ATK])), Times.Once);
            zoneManagerMock.Verify(zm => zm.MoveCardFromFieldToDeck(cardController), Times.Once);
            deckManagerMock.Verify(dm => dm.ShuffleDeck(Player.Player1), Times.Once);

            Assert.AreEqual(expectedHealth, player2Health);
        }

        [Test]
        public void CardController_Fight_CorrectlyDamagesOpponent()
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

            var opponentController = Container.Resolve<CardController.Factory>().Create(cardData2, Player.Player2);

            var factory = Container.Resolve<CardController.Factory>();
            var cardController = factory.Create(cardData1, Player.Player1);

            // Act
            IEnumerator enumerator = cardController.Fight(opponentController);
            while (enumerator.MoveNext()) { }

            // Assert
            Assert.AreEqual(-1, cardData1.StatBlock[StatBlocks.StatValues.DEF]);
            Assert.AreEqual(-1, cardData2.StatBlock[StatBlocks.StatValues.DEF]);
        }

        /*[Test]
        public void CardController_CheckDeath_MovesCardToGraveyard()
        {
            // Arrange
            CardData cardData = new CardData
            {
                StatBlock = new()
                {
                    Stats = new(0, 0, 0)
                }
            };

            var zoneManager = Container.Resolve<IZoneManager>();

            var factory = Container.Resolve<CardController.Factory>();
            var cardController = factory.Create(cardData, Player.Player1);


            // Act
            IEnumerator enumerator = cardController.CheckDeath();
            while (enumerator.MoveNext()) { }

            // Assert
            // Verify that the card has been moved to the graveyard
            Assert.AreEqual(ZoneType.Graveyard, cardData.Zone.Type);
            Assert.IsTrue(zoneManager.GetZoneCards(ZoneType.Graveyard).Contains(cardController));
        }

        [Test]
        public void CardController_Combat_PerformsCombatMovement()
        {
            // Arrange
            CardData cardData = new CardData
            {
                Owner = Player.Player1,
                StatBlock = new()
                {
                    Stats = new (0, 0, 1),
                }
            };

            var fieldManager = Container.Resolve<IFieldManager>();

            var factory = Container.Resolve<CardController.Factory>();
            var cardController = factory.Create(cardData, Player.Player1);

            // Act
            IEnumerator enumerator = cardController.Combat();
            while (enumerator.MoveNext()) { }

            // Assert
            // Verify that the combat movement has been performed
            Assert.AreEqual(1, fieldManager.GetCardMovement(cardController));
        }*/
    }
}
