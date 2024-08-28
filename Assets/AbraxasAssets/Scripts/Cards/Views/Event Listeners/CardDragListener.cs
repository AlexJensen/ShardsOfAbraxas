using Abraxas.Cards.Controllers;
using Abraxas.Cells.Views;
using Abraxas.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Abraxas.Cards.Views
{
    /// <summary>
    /// CardDragListener is a class that listens for drag events on a card and reports them to a DragHandler.
    /// </summary>
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
        PointerEventData _lastPointerData;
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
            _lastPointerData = eventData;
            _dragHandler.OnEndDrag();
        }

        public void DetermineLastDragRaycast()
        {
            List<RaycastResult> results = new();
            GraphicRaycaster.Raycast(_lastPointerData, results);
            foreach (var hit in results)
            {
                if (hit.gameObject.TryGetComponent<ICellView>(out var cell))
                {
                    if (Application.isPlaying)
                    {
                        StartCoroutine(_dragHandler.OnCardDraggedOverCell(cell.Controller));
                    }
                    else
                    {
                        var enumerator = _dragHandler.OnCardDraggedOverCell(cell.Controller);
                        Utilities.RunCoroutineToCompletion(enumerator);
                    }
                    return;
                }
            }
            if (Application.isPlaying)
            {
                StartCoroutine(_dragHandler.ReturnFromOverlayToHand());
            }
            else
            {
                var enumerator = _dragHandler.ReturnFromOverlayToHand();
                Utilities.RunCoroutineToCompletion(enumerator);
            }
        }
        #endregion
    }
}
