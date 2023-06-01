using Abraxas.Cards.Views;
using Abraxas.Cells.Controllers;
using Abraxas.Games.Managers;
using Abraxas.Manas;
using Abraxas.Players.Managers;
using Abraxas.Zones.Fields.Managers;
using Abraxas.Zones.Hands.Controllers;
using Abraxas.Zones.Hands.Managers;
using Abraxas.Zones.Overlays;
using System.Collections;

namespace Abraxas.Cards.Controllers
{
    class CardDragHandler : ICardDragHandler
    {
        #region Dependencies
        ICardController _cardController;
        ICardDragListener _cardDragListener;

        readonly Card.Settings.AnimationSettings _cardAnimationSettings;
        readonly IGameManager _gameManager;
        readonly IPlayerManager _playerManager;
        readonly IHandManager _handManager;
        readonly IFieldManager _fieldManager;
        readonly IManaManager _manaManager;
        public CardDragHandler(Card.Settings overlaySettings, IGameManager gameManager, IPlayerManager playerManager,
                               IHandManager handManager, IFieldManager fieldManager,
                               IManaManager manaManager)
        {
            _cardAnimationSettings = overlaySettings.animationSettings;
            _gameManager = gameManager;
            _playerManager = playerManager;
            _handManager = handManager;
            _fieldManager = fieldManager;
            _manaManager = manaManager;
        }

        internal void Initialize(ICardDragListener cardDragListener, ICardController cardController)
        {
            _cardDragListener = cardDragListener;
            _cardController = cardController;
        }
        #endregion

        #region Methods
        public void OnBeginDrag()
        {
            if (!_cardController.Hidden && _cardController.OriginalOwner == _playerManager.LocalPlayer && _cardController.Zone is IHandController)
            {
                _handManager.CardDragging = _cardController;
                _handManager.RemoveCard(_cardController);
                _cardController.AddToOverlay();
                _cardController.ScaleToRectangle(_fieldManager.GetCellDimensions(), _cardAnimationSettings.ScaleCardToOverlayTime);
            }
        }

        public void OnDrag()
        {
            if (_handManager.CardDragging == _cardController)
            {
                _cardController.View.SetCardPositionToMousePosition();
            }
        }

        public void OnEndDrag()
        {
            if (_handManager.CardDragging == _cardController)
            {
                _cardDragListener.DetermineDragRaycast();
            }
        }

        public IEnumerator OnCardDraggedOverCell(ICellController cell)
        {
            if (_handManager.CardDragging == _cardController && _cardController.OriginalOwner == _playerManager.ActivePlayer)
            {
                if (cell.CardsOnCell == 0 && cell.Player == _cardController.Owner && _manaManager.CanPurchaseCard(_cardController))
                {
                    _handManager.CardDragging = null;
                    _gameManager.RequestPurchaseCardAndMoveFromHandToCell(_cardController, cell.FieldPosition);
                    yield break;
                }
            }
            yield return ReturnFromOverlayToHand();
        }

        public IEnumerator ReturnFromOverlayToHand()
        {
            yield return _handManager.ReturnCardToHand(_cardController);
            _handManager.CardDragging = null;
        }
        #endregion
    }
}
