using Abraxas.Cards.Controllers;
using Abraxas.Cards.Data;
using Abraxas.Cards.Factories;
using Abraxas.Cards.Installers;
using Abraxas.Cards.Managers;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.CardViewers;
using Abraxas.Cells.Factories;
using Abraxas.Cells.Controllers;
using Abraxas.Cells.Models;
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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEditor.SceneManagement;
using Zenject;
using Player = Abraxas.Players.Players;



namespace Abraxas.Tests
{
    class TestInstaller : Installer<TestInstaller>
    {
        public override void InstallBindings()
        {
            CardSettingsInstaller.InstallFromResource("Settings/CardSettings", Container);
            StoneSettingsInstaller.InstallFromResource("Settings/StoneSettings", Container);
            PlayerSettingsInstaller.InstallFromResource("Settings/PlayerSettings", Container);

            Container.Bind<IGameManager>().FromMock();
            Container.Bind<IPlayerManager>().FromMock();
            Container.Bind<IHandManager>().FromMock();
            Container.Bind<IManaManager>().FromMock();
            Container.Bind<ICardManager>().FromMock();
            Container.Bind<ICardViewerManager>().FromMock();
            Container.Bind<IZoneManager>().FromMock();
            Container.Bind<IDeckManager>().FromMock();
            Container.Bind<IEventManager>().FromMock();
            Container.Bind<IPlayerHealthManager>().FromMock();
            Container.Bind<IFieldManager>().FromMock();
            Container.Bind<IOverlayManager>().FromMock();

            Container.BindInterfacesAndSelfTo<CardView>().FromNewComponentOnNewGameObject().AsTransient();
            Container.BindInterfacesAndSelfTo<CardController>().AsTransient();
            Container.BindInterfacesAndSelfTo<CardModel>().AsTransient();

            Container.BindFactory<CardData, ICardController, CardController.Factory>().FromFactory<CardFactory>();
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

        //[Test]
        //public void Create_CardWithDependencies_Success()
        //{
        //    // Arrange
        //    CardData cardData = new();

        //    var factory = Container.Resolve<CardController.Factory>();

        //    // Act
        //    var cardController = factory.Create(cardData);

        //    // Assert
        //    Assert.IsNotNull(cardController);
        //    Assert.IsNotNull(cardController.Model);
        //    Assert.IsNotNull(cardController.View);
        //}

        

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
            var cardController = cardFactory.Create(cardData1);
            var opponentController = cardFactory.Create(cardData2);

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
            var cardController = cardFactory.Create(cardData1);
            var opponentController = cardFactory.Create(cardData2);

            // Act
            IEnumerator enumerator = cardController.Fight(opponentController);
            while (enumerator.MoveNext()) { }

            // Assert
            Assert.AreEqual(0, cardController.StatBlock[StatBlocks.StatValues.DEF]);
            Assert.AreEqual(0, opponentController.StatBlock[StatBlocks.StatValues.DEF]);
        }

        [Test]
        public void CardController_PassHomeRow_ModifiesPlayerHealth()
        {
            //Bind
            Container.Unbind<IPlayerHealthManager>();
            Container.BindInterfacesAndSelfTo<PlayerHealthManager>().FromNewComponentOnNewGameObject().AsSingle();

            Container.BindInterfacesAndSelfTo<PlayerHealthView>().FromNewComponentOnNewGameObject().AsTransient();
            Container.BindInterfacesAndSelfTo<PlayerHealthController>().AsTransient();
            Container.BindInterfacesAndSelfTo<PlayerHealthModel>().AsTransient();

            Container.BindFactory<IPlayerHealthView, IPlayerHealthController, PlayerHealthController.Factory>().FromFactory<PlayerHealthFactory>();

            // Arrange
            CardData cardData = new()
            {
                Owner = Player.Player1,
                StatBlock = new()
                {
                    Stats = new(1, 0, 0)
                }
            };
            var healthManager = Container.Resolve<IPlayerHealthManager>();
            var healthFactory = Container.Resolve<PlayerHealthController.Factory>();
            var playerHealthViewMock = new Mock<IPlayerHealthView>();
            var playerHealthController = healthFactory.Create(playerHealthViewMock.Object);
            playerHealthController.MaxHP = 1;
            playerHealthController.HP = 1;
            playerHealthController.Player = Player.Player2;
            healthManager.AddPlayerHealth(playerHealthController);

            var cardFactory = Container.Resolve<CardController.Factory>();
            var cardController = cardFactory.Create(cardData);
            int expectedHealth = playerHealthController.HP - cardData.StatBlock[StatBlocks.StatValues.ATK];

            // Act
            IEnumerator enumerator = cardController.PassHomeRow();
            while (enumerator.MoveNext()) { }

            // Assert
            int player2Health = playerHealthController.HP;
            Assert.AreEqual(expectedHealth, player2Health);

            // Unbind
            Container.Unbind<IPlayerHealthManager>();
            Container.Bind<IPlayerHealthManager>().FromMock();
        }

        [Test]
        public void CardController_Combat_Player1MovesCardForward()
        {
            // Bind
            Container.Unbind<IFieldManager>();
            Container.BindInterfacesAndSelfTo<FieldManager>().FromNewComponentOnNewGameObject().AsSingle();
            Container.BindInterfacesAndSelfTo<FieldView>().FromNewComponentOnNewGameObject().AsTransient();
            Container.BindInterfacesAndSelfTo<FieldController>().AsTransient();
            Container.BindInterfacesAndSelfTo<FieldModel>().AsTransient();

            Container.BindInterfacesAndSelfTo<CellView>().FromNewComponentOnNewGameObject().AsTransient();
            Container.BindInterfacesAndSelfTo<CellController>().AsTransient();
            Container.BindInterfacesAndSelfTo<CellModel>().AsTransient();

            Container.BindFactory<ICellView, ICellController, CellController.Factory>().FromFactory<CellFactory>();

            Container.Bind(typeof(ZoneFactory<IFieldView, FieldController, FieldModel>)).ToSelf().AsTransient();

            // Arrange
            CardData cardData = new()
            {
                Owner = Player.Player1,
                StatBlock = new()
                {
                    Stats = new(0, 0, 1)
                }
            };

            var fieldViewMock = new Mock<IFieldView>();
            var cellFactory = Container.Resolve<CellController.Factory>();

            var fieldGrid = new List<List<ICellController>>();
            var row = new List<ICellController>();
            for (int i = 0; i < 2; i++)
            {
                var cellView = new Mock<ICellView>();
                var fieldPosition = new Point(i, 0);
                cellView.SetupGet(view => view.FieldPosition).Returns(fieldPosition);
                row.Add(cellFactory.Create(cellView.Object));
            }
            fieldGrid.Add(row);

            fieldViewMock.Setup(view => view.GenerateField()).Returns(fieldGrid);            

            var fieldManager = Container.Resolve<IFieldManager>();
            var fieldFactory = Container.Resolve<ZoneFactory<IFieldView, FieldController, FieldModel>>();
            var fieldController = fieldFactory.Create(fieldViewMock.Object);
            fieldManager.SetField(fieldController);

            var cardFactory = Container.Resolve<CardController.Factory>();
            var cardController = cardFactory.Create(cardData);
            fieldManager.AddCard(cardController, new Point(0,0));

            // Act
            IEnumerator enumerator = cardController.Combat();
            while (enumerator.MoveNext()) { }

            // Assert
            Assert.AreEqual(1, cardController.FieldPosition.X);
            Assert.AreEqual(0, cardController.FieldPosition.Y);

            // Unbind
            Container.Unbind<IFieldManager>();
            Container.Bind<IFieldManager>().FromMock();
        }
    }
}
