using Abraxas.Cards.Controllers;
using Abraxas.Cards.Data;
using Abraxas.Cards.Factories;
using Abraxas.Cards.Installers;
using Abraxas.Cards.Managers;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.CardViewers;
using Abraxas.Cells.Controllers;
using Abraxas.Cells.Factories;
using Abraxas.Cells.Models;
using Abraxas.Cells.Views;
using Abraxas.Core;
using Abraxas.Events;
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
using Abraxas.Random.Managers;
using Abraxas.StatBlocks;
using Abraxas.StatBlocks.Controllers;
using Abraxas.StatBlocks.Data;
using Abraxas.StatBlocks.Factories;
using Abraxas.StatBlocks.Installers;
using Abraxas.StatBlocks.Views;
using Abraxas.Stones;
using Abraxas.Stones.Controllers;
using Abraxas.Stones.Data;
using Abraxas.Stones.Factories;
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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using Zenject;
using Player = Abraxas.Players.Players;



namespace Abraxas.Tests
{
    class CardTestInstaller : Installer<CardTestInstaller>
    {
        public override void InstallBindings()
        {
            CardSettingsInstaller.InstallFromResource("Settings/Test/CardSettings", Container);
            StoneSettingsInstaller.InstallFromResource("Settings/Test/StoneSettings", Container);
            PlayerSettingsInstaller.InstallFromResource("Settings/Test/PlayerSettings", Container);
            StatBlockSettingsInstaller.InstallFromResource("Settings/Test/StatblockSettings", Container);

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
            Container.Bind<IRandomManager>().FromMock();

            Container.BindInterfacesAndSelfTo<CardView>().FromNewComponentOnNewGameObject().AsTransient();
            Container.BindInterfacesAndSelfTo<CardController>().AsTransient();
            Container.BindInterfacesAndSelfTo<CardModel>().AsTransient();

            Container.BindFactory<CardData, ICardController, CardController.Factory>().FromFactory<CardFactory>();
            Container.BindFactory<StoneDataSO, IStoneController, StoneController.Factory>().FromFactory<StoneFactory>();
            Container.BindFactory<StatBlockData, IStatBlockView, IStatBlockController, StatBlockController.Factory>().FromFactory<StatBlockFactory>();
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
            CardTestInstaller.Install(Container);
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
        public void Create_Card_Success()
        {
            // Arrange
            CardData cardData = new();


            var factory = Container.Resolve<CardController.Factory>();

            // Act
            var cardController = factory.Create(cardData);


            // Assert
            Assert.IsNotNull(cardController);
        }

        [Test]
        public void Create_CardWithData_Success()
        {
            CardData cardData = new()
            {
                StatBlock = new()
                {
                    Cost = 1,
                    StoneType = StoneType.GARNET,
                    Stats = new()
                    {
                        ATK = 1,
                        DEF = 1,
                        SPD = 1,
                        RNG = 1,
                    }
                }
            };

            var factory = Container.Resolve<CardController.Factory>();
            var cardController = factory.Create(cardData);

            Assert.NotNull(cardController);
            Assert.AreEqual(1, cardController.StatBlock.Stats.ATK);
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
                    Stats = new()
                    {
                        ATK = 1,
                        DEF = 0,
                        SPD = 0,
                        RNG = 0,
                    }
                }
            };

            CardData cardData2 = new()
            {
                Owner = Player.Player2,
                StatBlock = new()
                {
                    Stats = new()
                    {
                        ATK = 1,
                        DEF = 0,
                        SPD = 0,
                        RNG = 0,
                    }
                }
            };

            var cardFactory = Container.Resolve<CardController.Factory>();
            var cardController = cardFactory.Create(cardData1);
            var opponentController = cardFactory.Create(cardData2);

            // Act
            Utilities.RunCoroutineToCompletion(cardController.Fight(opponentController));

            // Assert
            Assert.AreEqual(-1, cardController.StatBlock.Stats.DEF);
            Assert.AreEqual(-1, opponentController.StatBlock.Stats.DEF);
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
                    Stats = new()
                    {
                        ATK = 1,
                        DEF = 0,
                        SPD = 0,
                        RNG = 0,
                    }
                }
            };

            CardData cardData2 = new()
            {
                Owner = Player.Player1,
                StatBlock = new()
                {
                    Stats = new()
                    {
                        ATK = 1,
                        DEF = 0,
                        SPD = 0,
                        RNG = 0,
                    }
                }
            };

            var cardFactory = Container.Resolve<CardController.Factory>();
            var cardController = cardFactory.Create(cardData1);
            var opponentController = cardFactory.Create(cardData2);

            // Act
            Utilities.RunCoroutineToCompletion(cardController.Fight(opponentController));

            // Assert
            Assert.AreEqual(0, cardController.StatBlock.Stats.DEF);
            Assert.AreEqual(0, opponentController.StatBlock.Stats.DEF);
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
                    Stats = new()
                    {
                        ATK = 1,
                        DEF = 0,
                        SPD = 0,
                        RNG = 0,
                    }
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
            int expectedHealth = playerHealthController.HP - cardData.StatBlock.Stats.ATK;

            // Act
            Utilities.RunCoroutineToCompletion(cardController.PassHomeRow());

            // Assert
            int player2Health = playerHealthController.HP;
            Assert.AreEqual(expectedHealth, player2Health);

            // Unbind
            Container.Unbind<IPlayerHealthManager>();
            Container.Bind<IPlayerHealthManager>().FromMock();
        }

        [Test]

        public void CardController_Combat_MovesCardForward()
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
                    Stats = new()
                    {
                        ATK = 1,
                        DEF = 0,
                        SPD = 1,
                        RNG = 0,
                    }
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


            Utilities.RunCoroutineToCompletion(fieldManager.MoveCardToCell(cardController, new Point(0, 0)));

            Assert.AreEqual(0, cardController.Cell.FieldPosition.X);
            Assert.AreEqual(0, cardController.Cell.FieldPosition.Y);

            // Act
            Utilities.RunCoroutineToCompletion(cardController.Combat());


            // Assert
            Assert.AreEqual(1, cardController.Cell.FieldPosition.X);
            Assert.AreEqual(0, cardController.Cell.FieldPosition.Y);

            // Unbind
            Container.Unbind<IFieldManager>();
            Container.Bind<IFieldManager>().FromMock();
        }

        [Test]
        public void CardController_Initialization_Success()
        {
            // Arrange
            var modelMock = new Mock<ICardModel>();
            var viewMock = new Mock<ICardView>();
            var eventManagerMock = new Mock<IEventManager>();

            var cardController = new CardController(
                Container.Resolve<IZoneManager>(),
                Container.Resolve<IDeckManager>(),
                eventManagerMock.Object,
                Container.Resolve<IPlayerHealthManager>(),
                Container.Resolve<IFieldManager>());

            // Act
            cardController.Initialize(modelMock.Object, viewMock.Object);

            // Assert
            eventManagerMock.Verify(em => em.AddListener<ManaModifiedEvent>(typeof(ManaModifiedEvent), cardController), Times.Once);
            eventManagerMock.Verify(em => em.AddListener<CardChangedZonesEvent>(typeof(CardChangedZonesEvent), cardController), Times.Once);

            var modelField = typeof(CardController).GetField("_model", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var viewField = typeof(CardController).GetField("_view", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            Assert.AreSame(modelMock.Object, modelField.GetValue(cardController));
            Assert.AreSame(viewMock.Object, viewField.GetValue(cardController));
        }

        [Test]
        public void CardController_EventListeners_AddedAndRemoved()
        {
            // Arrange
            var modelMock = new Mock<ICardModel>();
            var viewMock = new Mock<ICardView>();
            var eventManagerMock = new Mock<IEventManager>();

            var cardController = new CardController(
                Container.Resolve<IZoneManager>(),
                Container.Resolve<IDeckManager>(),
                eventManagerMock.Object,
                Container.Resolve<IPlayerHealthManager>(),
                Container.Resolve<IFieldManager>());

            // Act
            cardController.Initialize(modelMock.Object, viewMock.Object);
            cardController.OnDestroy();

            // Assert
            eventManagerMock.Verify(em => em.AddListener<ManaModifiedEvent>(typeof(ManaModifiedEvent), cardController), Times.Once);
            eventManagerMock.Verify(em => em.AddListener<CardChangedZonesEvent>(typeof(CardChangedZonesEvent), cardController), Times.Once);
            eventManagerMock.Verify(em => em.RemoveListener<ManaModifiedEvent>(typeof(ManaModifiedEvent), cardController), Times.Once);
            eventManagerMock.Verify(em => em.RemoveListener<CardChangedZonesEvent>(typeof(CardChangedZonesEvent), cardController), Times.Once);
        }


        [Test]
        public void CardController_PropertyChangeTests()
        {
            // Arrange
            var cardController = Container.Instantiate<CardController>();
            var modelMock = new Mock<ICardModel>();
            var viewMock = new Mock<ICardView>();

            cardController.Initialize(modelMock.Object, viewMock.Object);

            // Act & Assert
            cardController.Title = "New Title";
            modelMock.VerifySet(m => m.Title = "New Title");

            cardController.Owner = Player.Player2;
            modelMock.VerifySet(m => m.Owner = Player.Player2);

            cardController.Hidden = true;
            modelMock.VerifySet(m => m.Hidden = true);
        }

        [Test]
        public void CardController_GetCostText_ValidCostText()
        {
            // Arrange
            var modelMock = new Mock<ICardModel>();
            var viewMock = new Mock<ICardView>();
            var playerSettingsMock = new Mock<Players.Player.Settings>();
            var statblockSettingsMock = new Mock<Statblock.Settings>();

            var cardController = new CardController(
                Container.Resolve<IZoneManager>(),
                Container.Resolve<IDeckManager>(),
                Container.Resolve<IEventManager>(),
                Container.Resolve<IPlayerHealthManager>(),
                Container.Resolve<IFieldManager>());

            cardController.Initialize(modelMock.Object, viewMock.Object);

            var stoneSettings = Container.Resolve<Stone.Settings>();

            var stoneDetails1 = stoneSettings.GetStoneTypeDetails(StoneType.AMETHYST);
            var stoneDetails2 = stoneSettings.GetStoneTypeDetails(StoneType.SAPPHIRE);

            var totalCosts = new Dictionary<StoneType, int>
            {
                { StoneType.AMETHYST, 1 },
                { StoneType.SAPPHIRE, 2 }
            };

            modelMock.Setup(m => m.TotalCosts).Returns(totalCosts);

            viewMock.Setup(v => v.GetCostText()).Returns(() =>
            {
                var costStrings = totalCosts
                    .Where(pair => pair.Value != 0)
                    .Select(pair => $"<#{ColorUtility.ToHtmlStringRGB(stoneSettings.GetStoneTypeDetails(pair.Key).color)}>{pair.Value}");

                return string.Join("", costStrings);
            });

            // Act
            var costText = cardController.GetCostText();

            // Assert
            Assert.AreEqual($"<#7F00FF>1<#0043FF>2", costText);
        }
    }
}
