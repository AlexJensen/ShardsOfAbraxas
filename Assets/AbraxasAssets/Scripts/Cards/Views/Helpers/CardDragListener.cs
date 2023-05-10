using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Abraxas.Zones.Fields;
using Abraxas.Cards.Controllers;

namespace Abraxas.Cards.Views
{
    [RequireComponent(typeof(ICardView))]
    public class CardDragListener : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        #region Fields
        ICardDragHandler _dragHandler;
        Canvas _canvas;
        GraphicRaycaster _graphicRaycaster;
        #endregion

        #region Properties
        public Canvas Canvas => _canvas = _canvas != null ? _canvas : GetComponentInParent<Canvas>();
        public GraphicRaycaster GraphicRaycaster => _graphicRaycaster = _graphicRaycaster != null ? _graphicRaycaster : Canvas.GetComponent<GraphicRaycaster>();
        public ICardDragHandler DragHandler => _dragHandler != null? _dragHandler : GetComponent<ICardView>().Controller.DragHandler;
        #endregion

        #region Methods
        public void OnBeginDrag(PointerEventData eventData)
        {
            DragHandler.OnBeginDrag();
        }

        public void OnDrag(PointerEventData eventData)
        {
            DragHandler.OnDrag();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            List<RaycastResult> results = new();
            GraphicRaycaster.Raycast(eventData, results);
            foreach (var hit in results)
            {
                ICellView cell = hit.gameObject.GetComponent<ICellView>();
                if (cell != null)
                {
                    DragHandler.OnCardDraggedOverCell(cell.Controller);
                    return;
                }
            }
            StartCoroutine(DragHandler.ReturnFromOverlayToHand());
        }
        #endregion
    }
}
