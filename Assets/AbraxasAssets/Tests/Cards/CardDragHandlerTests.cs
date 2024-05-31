using Abraxas.Cards.Controllers;
using Abraxas.Cards.Views;
using Abraxas.Cards;
using Abraxas.Cells.Controllers;
using Abraxas.Core;
using Abraxas.Games.Managers;
using Abraxas.Manas;
using Abraxas.Players.Managers;
using Abraxas.Zones.Fields.Managers;
using Abraxas.Zones.Hands.Controllers;
using Abraxas.Zones.Hands.Managers;
using Abraxas.Zones.Overlays.Managers;
using Moq;
using NUnit.Framework;
using Zenject;
using Player = Abraxas.Players.Players;
using System.Drawing;

namespace Abraxas.Tests
{
    [TestFixture]
    public class CardDragHandlerTests : ZenjectUnitTestFixture
    {
        private Mock<ICardController> _cardControllerMock;
        private Mock<ICardDragListener> _cardDragListenerMock;
        private Mock<IOverlayManager> _overlayManagerMock;
        private Mock<IGameManager> _gameManagerMock;
        private Mock<IPlayerManager> _playerManagerMock;
        private Mock<IHandManager> _handManagerMock;
        private Mock<IFieldManager> _fieldManagerMock;
        private Mock<IManaManager> _manaManagerMock;
        private Mock<Card.Settings> _cardSettings;
        private CardDragHandler _cardDragHandler;

        [SetUp]
        public void SetUp()
        {
            _cardControllerMock = new Mock<ICardController>();
            _cardDragListenerMock = new Mock<ICardDragListener>();
            _overlayManagerMock = new Mock<IOverlayManager>();
            _gameManagerMock = new Mock<IGameManager>();
            _playerManagerMock = new Mock<IPlayerManager>();
            _handManagerMock = new Mock<IHandManager>();
            _fieldManagerMock = new Mock<IFieldManager>();
            _manaManagerMock = new Mock<IManaManager>();
            _cardSettings = new Mock<Card.Settings>();

            _cardDragHandler = new CardDragHandler(_cardSettings.Object, _gameManagerMock.Object, _overlayManagerMock.Object, _playerManagerMock.Object, _handManagerMock.Object, _fieldManagerMock.Object, _manaManagerMock.Object);
            _cardDragHandler.Initialize(_cardDragListenerMock.Object, _cardControllerMock.Object);
        }

        [Test]
        public void OnBeginDrag_CardNotHidden_CardIsSetForDragging()
        {
            // Arrange
            _cardControllerMock.Setup(c => c.Hidden).Returns(false);
            _cardControllerMock.Setup(c => c.OriginalOwner).Returns(Player.Player1);
            _playerManagerMock.Setup(p => p.LocalPlayer).Returns(Player.Player1);
            _cardControllerMock.Setup(c => c.Zone).Returns(Mock.Of<IHandController>());

            // Act
            _cardDragHandler.OnBeginDrag();

            // Assert
            _handManagerMock.Verify(h => h.RemoveCard(_cardControllerMock.Object), Times.Once);
            _overlayManagerMock.Verify(o => o.SetCard(_cardControllerMock.Object), Times.Once);
            _cardControllerMock.Verify(c => c.ChangeScale(It.IsAny<PointF>(), It.IsAny<float>()), Times.Once);
        }

        [Test]
        public void OnDrag_CardIsDragging_PositionIsSetToMousePosition()
        {
            // Arrange
            _handManagerMock.Setup(h => h.CardDragging).Returns(_cardControllerMock.Object);

            // Act
            _cardDragHandler.OnDrag();

            // Assert
            _cardControllerMock.Verify(c => c.SetCardPositionToMousePosition(), Times.Once);
        }

        [Test]
        public void OnEndDrag_CardIsDragging_DetermineLastDragRaycastIsCalled()
        {
            // Arrange
            _handManagerMock.Setup(h => h.CardDragging).Returns(_cardControllerMock.Object);

            // Act
            _cardDragHandler.OnEndDrag();

            // Assert
            _cardDragListenerMock.Verify(c => c.DetermineLastDragRaycast(), Times.Once);
        }

        [Test]
        public void OnCardDraggedOverCell_CardCanBePurchased_CardMovedToCell()
        {
            // Arrange
            _handManagerMock.Setup(h => h.CardDragging).Returns(_cardControllerMock.Object);
            _playerManagerMock.Setup(p => p.ActivePlayer).Returns(Player.Player1);
            _cardControllerMock.Setup(c => c.OriginalOwner).Returns(Player.Player1);
            var cellMock = new Mock<ICellController>();
            cellMock.Setup(c => c.CardsOnCell).Returns(0);
            cellMock.Setup(c => c.Player).Returns(Player.Player1);
            _manaManagerMock.Setup(m => m.CanPurchaseCard(_cardControllerMock.Object)).Returns(true);

            // Act
            Utilities.RunCoroutineToCompletion(_cardDragHandler.OnCardDraggedOverCell(cellMock.Object));

            // Assert
            _handManagerMock.VerifySet(h => h.CardDragging = null, Times.Once);
            _gameManagerMock.Verify(g => g.RequestPurchaseCardAndMoveFromHandToCell(_cardControllerMock.Object, cellMock.Object.FieldPosition), Times.Once);
        }

        [Test]
        public void OnCardDraggedOverCell_CardCannotBePurchased_CardReturnedToHand()
        {
            // Arrange
            _handManagerMock.Setup(h => h.CardDragging).Returns(_cardControllerMock.Object);
            _playerManagerMock.Setup(p => p.ActivePlayer).Returns(Player.Player1);
            _cardControllerMock.Setup(c => c.OriginalOwner).Returns(Player.Player1);
            var cellMock = new Mock<ICellController>();
            cellMock.Setup(c => c.CardsOnCell).Returns(1);

            // Act
            Utilities.RunCoroutineToCompletion(_cardDragHandler.OnCardDraggedOverCell(cellMock.Object));

            // Assert
            _handManagerMock.Verify(h => h.ReturnCardToHand(_cardControllerMock.Object), Times.Once);
            _handManagerMock.VerifySet(h => h.CardDragging = null, Times.Once);
        }

        [Test]
        public void ReturnFromOverlayToHand_CardReturnedToHand()
        {
            // Act
            Utilities.RunCoroutineToCompletion(_cardDragHandler.ReturnFromOverlayToHand());

            // Assert
            _handManagerMock.Verify(h => h.ReturnCardToHand(_cardControllerMock.Object), Times.Once);
            _handManagerMock.VerifySet(h => h.CardDragging = null, Times.Once);
        }
    }
}
