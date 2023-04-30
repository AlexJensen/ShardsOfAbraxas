using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using Abraxas.Players;
using Abraxas.Manas;
using Abraxas.Zones.Fields;
using Abraxas.Zones.Overlays;

using Zone = Abraxas.Zones.Zones;
using Abraxas.Zones.Hands;
using Abraxas.Game;

namespace Abraxas.Cards
{
    [RequireComponent(typeof(Card))]
    public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        #region Dependencies
        Card.Settings _cardSettings;
        IGameManager _gameManager;
        IPlayerManager _playerManager;
        IManaManager _manaManager;
        IOverlayManager _overlayManager;
        IFieldManager _fieldManager;
        IHandManager _handManager;
        [Inject]
        public void Construct(Card.Settings cardSettings, IGameManager gameManager, IPlayerManager playerManager, IManaManager manaManager, IOverlayManager overlayManager, IFieldManager fieldManager, IHandManager handManager)
        {
            _cardSettings = cardSettings;
            _gameManager = gameManager;
            _playerManager = playerManager;
            _manaManager = manaManager;
            _overlayManager = overlayManager;
            _fieldManager = fieldManager;
            _handManager = handManager;
        }
        #endregion

        #region Fields
        Card _card;
        Canvas _canvas;
        GraphicRaycaster _graphicRaycaster;
        #endregion

        #region Properties
        public Card Card => _card = _card != null ? _card : GetComponent<Card>();
        public RectTransform RectTransform => (RectTransform)transform;
        public Canvas Canvas => _canvas = _canvas != null ? _canvas : GetComponentInParent<Canvas>();
        public GraphicRaycaster GraphicRaycaster => _graphicRaycaster = _graphicRaycaster != null ? _graphicRaycaster : Canvas.GetComponent<GraphicRaycaster>();
        #endregion

        #region Methods
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!Card.Hidden && Card.Owner == _playerManager.ActivePlayer && Card.Zone == Zone.HAND)
            {
                _handManager.RemoveCard(Card.Owner, Card);
                _overlayManager.AddCard(Card);
                StartCoroutine(Card.RectTransformMover.ChangeScaleEnumerator(_fieldManager.GetCellDimensions(), _cardSettings.ScaleToFieldCellTime));
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_overlayManager.Card == Card)
            {
                RectTransform.position = Input.mousePosition;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_overlayManager.Card == Card)
            {
                List<RaycastResult> results = new();
                GraphicRaycaster.Raycast(eventData, results);
                foreach (var hit in results)
                {
                    Cell cell = hit.gameObject.GetComponent<Cell>();
                    if (cell)
                    {
                        if (cell.Cards.Count == 0 && cell.Player == Card.Controller && _manaManager.CanPurchaseCard(Card))
                        {
                            _overlayManager.RemoveCard(Card);
                            _gameManager.RequestPurchaseCardAndMoveFromHandToCell(Card, cell.FieldPosition);
                            return;
                        }
                    }
                }
                StartCoroutine(_handManager.ReturnCardToHand(Card));
                _overlayManager.RemoveCard(Card);
            }
        }
        #endregion
    }
}
