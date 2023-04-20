using Abraxas.Game;
using Abraxas.Zones.Fields;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using Zone = Abraxas.Zones.Zones;
using Unity.Netcode;

namespace Abraxas.Cards
{
    [RequireComponent(typeof(Card))]
    public class CardDragHandler : NetworkBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        #region Dependencies
        IGameManager _gameManager;
        [Inject]
        public void Construct(IGameManager gameManager)
        {
            _gameManager = gameManager;
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
            if (!Card.Hidden)
            {
                switch (Card.Zone)
                {
                    case Zone.HAND:
                        {
                            
                        }
                        break;
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            switch (Card.Zone)
            {
                case Zone.DRAG:
                    {
                        RectTransform.position = Input.mousePosition;
                    }
                    break;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            switch (Card.Zone)
            {
                case Zone.DRAG:
                    {
                        List<RaycastResult> results = new();
                        GraphicRaycaster.Raycast(eventData, results);
                        foreach (var hit in results)
                        {
                            Cell cell = hit.gameObject.GetComponent<Cell>();
                            if (cell)
                            {
                                if (cell.Cards.Count == 0 && cell.Player == Card.Controller && _gameManager.CanPurchaseCard(Card))
                                {

                                    _gameManager.PurchaseCard(Card);
                                    Card.RequestMoveToCellServerRpc(cell.FieldPosition);
                                    return;
                                }
                            }
                        }
                        StartCoroutine(_gameManager.MoveCardToHand(Card));
                    }
                    break;
            }
        }
        #endregion
    }
}
