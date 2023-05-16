using Abraxas.Cells.Controllers;
using Abraxas.Game.Managers;
using Abraxas.Manas;
using Abraxas.Players.Managers;
using Abraxas.Zones.Fields.Managers;
using Abraxas.Zones.Hands.Controllers;
using Abraxas.Zones.Hands.Managers;
using Abraxas.Zones.Overlays.Managers;
using System.Collections;

namespace Abraxas.Cards.Controllers
{
    class CardDragHandler : ICardDragHandler
    {
        #region Dependencies
        ICardController _cardController;

        readonly Overlay.Settings _overlaySettings;
        readonly IGameManager _gameManager;
        readonly IPlayerManager _playerManager;
        readonly IHandManager _handManager;
        readonly IOverlayManager _overlayManager;
        readonly IFieldManager _fieldManager;
        readonly IManaManager _manaManager;
        public CardDragHandler(Overlay.Settings overlaySettings, IGameManager gameManager, IPlayerManager playerManager,
                               IHandManager handManager, IOverlayManager overlayManager, IFieldManager fieldManager,
                               IManaManager manaManager)
        {
            _overlaySettings = overlaySettings;
            _gameManager = gameManager;
            _playerManager = playerManager;
            _handManager = handManager;
            _overlayManager = overlayManager;
            _fieldManager = fieldManager;
            _manaManager = manaManager;
        }

        internal void Initialize(ICardController cardController)
        {
            _cardController = cardController;
        }
        #endregion

        #region Methods
        public void OnBeginDrag()
        {
            if (!_cardController.Hidden && _cardController.OriginalOwner == _playerManager.LocalPlayer && _cardController.Zone is IHandController)
            {
                _handManager.RemoveCard(_cardController.OriginalOwner, _cardController);
                _cardController.View.ChangeScale(_fieldManager.GetCellDimensions(), _overlaySettings.ScaleCardToOverlayTime);
            }
        }

        public void OnDrag()
        {
            if (_overlayManager.Card == _cardController.View)
            {
                _cardController.View.SetCardPositionToMousePosition();
            }
        }

        public IEnumerator OnCardDraggedOverCell(ICellController cell)
        {
            if (_overlayManager.Card == _cardController.View && _cardController.OriginalOwner == _playerManager.ActivePlayer)
            {
                if (cell.CardsOnCell == 0 && cell.Player == _cardController.Owner && _manaManager.CanPurchaseCard(_cardController))
                {
                    _gameManager.RequestPurchaseCardAndMoveFromHandToCell(_cardController, cell.FieldPosition);
                    yield break;
                }
            }
            yield return ReturnFromOverlayToHand();
        }

        public IEnumerator ReturnFromOverlayToHand()
        {
            yield return _handManager.ReturnCardToHand(_cardController);
        }
        #endregion
    }
}
