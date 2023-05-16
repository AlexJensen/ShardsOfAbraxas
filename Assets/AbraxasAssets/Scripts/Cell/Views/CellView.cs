using Abraxas.Cells.Controllers;
using System.Drawing;
using UnityEngine;
using Player = Abraxas.Players.Players;

namespace Abraxas.Cells.Views
{
    class CellView : MonoBehaviour, ICellView
    {
        #region Dependencies
        ICellController _controller;

        public void Initialize(ICellController controller)
        {
            _controller = controller;
        }
        #endregion

        #region Fields
        [SerializeField]
        Player _player;
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

        public Point FieldPosition => new(transform.parent.GetSiblingIndex(), transform.GetSiblingIndex());

        public Player Player { get => _player; }
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