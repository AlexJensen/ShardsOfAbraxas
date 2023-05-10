using Abraxas.Game;
using Abraxas.Manas;
using Abraxas.Players;
using Abraxas.Zones.Fields;
using Abraxas.Zones.Hands;
using Abraxas.Zones.Overlays;
using System.Collections;
using Zone = Abraxas.Zones.Zones;

namespace Abraxas.Cards.Controllers
{
    class CardDragHandler : ICardDragHandler
    {
        #region Dependencies
        readonly ICardController _cardController;

        readonly Overlay.Settings _overlaySettings;
        readonly IGameManager _gameManager;
        readonly IPlayerManager _playerManager;
        readonly IHandManager _handManager;
        readonly IOverlayManager _overlayManager;
        readonly IFieldManager _fieldManager;
        readonly IManaManager _manaManager;
        public CardDragHandler(ICardController cardController, Overlay.Settings overlaySettings, IGameManager gameManager, IPlayerManager playerManager,
                               IHandManager handManager, IOverlayManager overlayManager, IFieldManager fieldManager,
                               IManaManager manaManager)
        {
            _cardController = cardController;
            _overlaySettings = overlaySettings;
            _gameManager = gameManager;
            _playerManager = playerManager;
            _handManager = handManager;
            _overlayManager = overlayManager;
            _fieldManager = fieldManager;
            _manaManager = manaManager;
        }
        #endregion

        #region Fields

        #endregion

        #region Methods
        public void OnBeginDrag()
        {
            if (!_cardController.Hidden && _cardController.OriginalOwner == _playerManager.LocalPlayer && _cardController.Zone == Zone.HAND)
            {
                _handManager.RemoveCard(_cardController.OriginalOwner, _cardController);
                _overlayManager.AddCard(_cardController);
                _cardController.View.ChangeScale(_fieldManager.GetCellDimensions(), _overlaySettings.ScaleCardToOverlayTime);
            }
        }

        public void OnDrag()
        {
            if (_overlayManager.Card == this)
            {
                _cardController.View.SetCardPositionToMousePosition();
            }
        }

        public void OnCardDraggedOverCell(ICellController cell)
        {
            if (_overlayManager.Card == this && _cardController.OriginalOwner == _playerManager.ActivePlayer)
            {
                if (cell.Cards.Count == 0 && cell.Player == _cardController.Owner && _manaManager.CanPurchaseCard(_cardController))
                {
                    _overlayManager.RemoveCard(_cardController);
                    _gameManager.RequestPurchaseCardAndMoveFromHandToCell(_cardController, cell.FieldPosition);
                    return;
                }
            }
        }

        public IEnumerator ReturnFromOverlayToHand()
        {
            yield return _handManager.ReturnCardToHand(_cardController);
            _overlayManager.RemoveCard(_cardController);
        }
        #endregion
    }
}
