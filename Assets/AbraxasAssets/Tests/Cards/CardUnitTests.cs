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
using Abraxas.Manas;
using Abraxas.Players.Installers;
using Abraxas.Players.Managers;
using Abraxas.Zones.Decks.Managers;
using Abraxas.Zones.Fields.Managers;
using Abraxas.Zones.Hands.Managers;
using Abraxas.Zones.Managers;
using Abraxas.Zones.Overlays.Managers;
using NUnit.Framework;
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
            var cardData = new CardData();
            var player = Player.Player1;

            var factory = Container.Resolve<CardController.Factory>();

            // Act
            var cardController = factory.Create(cardData, player);

            // Assert
            Assert.IsNotNull(cardController);
            Assert.IsNotNull(cardController.Model);
            Assert.IsNotNull(cardController.View);
        }
    }
}
