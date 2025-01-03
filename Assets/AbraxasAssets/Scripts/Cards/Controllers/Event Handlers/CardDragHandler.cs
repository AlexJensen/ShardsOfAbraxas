﻿using Abraxas.Cards.Views;
using Abraxas.Cells.Controllers;
using Abraxas.Games.Managers;
using Abraxas.Manas;
using Abraxas.Players.Managers;
using Abraxas.Zones.Fields.Managers;
using Abraxas.Zones.Hands.Controllers;
using Abraxas.Zones.Hands.Managers;
using Abraxas.Zones.Overlays.Managers;
using System.Collections;

namespace Abraxas.Cards.Controllers
{
    /// <summary>
    /// CardDragHandler is a controller-level class for handling card dragging events.
    /// </summary>
    class CardDragHandler : ICardDragHandler
    {
        #region Dependencies
        ICardController _cardController;
        ICardDragListener _cardDragListener;

        readonly Card.Settings.AnimationSettings _cardAnimationSettings;
        readonly IOverlayManager _overlayManager;
        readonly IGameManager _gameManager;
        readonly IPlayerManager _playerManager;
        readonly IHandManager _handManager;
        readonly IFieldManager _fieldManager;
        readonly IManaManager _manaManager;
        public CardDragHandler(Card.Settings overlaySettings, IGameManager gameManager, IOverlayManager overlayManager, IPlayerManager playerManager,
                               IHandManager handManager, IFieldManager fieldManager, IManaManager manaManager)
        {
            _cardAnimationSettings = overlaySettings.animationSettings;
            _overlayManager = overlayManager;
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
                _overlayManager.SetCard(_cardController);
                _fieldManager.HighlightPlayableOpenCells(_cardController);
                _cardController.ChangeScale(_fieldManager.GetCellDimensions(), _cardAnimationSettings.ScaleCardToOverlayTime);
                _cardController.StatBlock.ShowSymbols = true;
            }
        }

        public void OnDrag()
        {
            if (_handManager.CardDragging == _cardController)
            {
                _cardController.SetCardPositionToMousePosition();
            }
        }

        public void OnEndDrag()
        {
            _fieldManager.SetHighlightVisible(false);
            if (_handManager.CardDragging == _cardController)
            {
                _cardDragListener.DetermineLastDragRaycast();
            }
        }

        public IEnumerator OnCardDraggedOverCell(ICellController cell)
        {
            if (_handManager.CardDragging == _cardController && _cardController.DeterminePlayability())
            {
                if (_cardController.IsCellAvailable(cell) && _manaManager.CanPurchaseCard(_cardController))
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
            _cardController.StatBlock.ShowSymbols = false;
            yield return _handManager.ReturnCardToHand(_cardController);
            _handManager.CardDragging = null;
        }
        #endregion
    }
}
