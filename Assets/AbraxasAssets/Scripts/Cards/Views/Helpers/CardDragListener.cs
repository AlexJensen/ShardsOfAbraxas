using Abraxas.Cards.Controllers;
using Abraxas.Cells.Views;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Abraxas.Cards.Views
{
    [RequireComponent(typeof(ICardView))]
    public class CardDragListener : MonoBehaviour, ICardDragListener, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        #region Dependencies
        ICardDragHandler _dragHandler;
        internal void Initialize(ICardDragHandler cardDragHandler)
        {
            _dragHandler = cardDragHandler;
        }
        #endregion

        #region Fields
        Canvas _canvas;
        GraphicRaycaster _graphicRaycaster;
        PointerEventData lastPointerData;
        #endregion

        #region Properties
        public Canvas Canvas => _canvas = _canvas != null ? _canvas : GetComponentInParent<Canvas>();
        public GraphicRaycaster GraphicRaycaster => _graphicRaycaster = _graphicRaycaster != null ? _graphicRaycaster : Canvas.GetComponent<GraphicRaycaster>();
        #endregion

        #region Methods
        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragHandler.OnBeginDrag();
        }

        public void OnDrag(PointerEventData eventData)
        {
            _dragHandler.OnDrag();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            lastPointerData = eventData;
            _dragHandler.OnEndDrag();
        }

        public void DetermineLastDragRaycast()
        {
            List<RaycastResult> results = new();
            GraphicRaycaster.Raycast(lastPointerData, results);
            foreach (var hit in results)
            {
                ICellView cell = hit.gameObject.GetComponent<ICellView>();
                if (cell != null)
                {
                    StartCoroutine(_dragHandler.OnCardDraggedOverCell(cell.Controller));
                    return;
                }
            }
            StartCoroutine(_dragHandler.ReturnFromOverlayToHand());
        }
        #endregion
    }
}
