using Abraxas.Cards.Views;
using UnityEngine;
using Zenject;

namespace Abraxas.Zones.Fields
{
    class CellView : MonoBehaviour, ICellView
    {
        #region Dependencies
        ICellController _controller;
        [Inject]
        public void Construct(ICellController controller)
        {
            _controller = controller;
        }
        #endregion

        #region Properties
        public ICellController Controller 
        { 
            get 
            { 
                return _controller; 
            } 
        }
        public RectTransform RectTransform { get => (RectTransform)transform; }
        #endregion

        #region Methods
        public void FitRectTransformInCell(RectTransform rect)
        {
            rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, RectTransform.rect.width);
            rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, RectTransform.rect.height);
        }

        public void SetChild(Transform transform)
        {
            transform.SetParent(transform);
            FitRectTransformInCell((RectTransform)transform);
        }
        #endregion
    }
}