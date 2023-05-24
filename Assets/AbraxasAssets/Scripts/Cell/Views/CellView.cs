using Abraxas.Cells.Controllers;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
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

            _cellBack.sprite = _cellBackTextures[Random.Range(0, _cellBackTextures.Length)];
        }
        #endregion

        #region Fields
        [SerializeField]
        Player _player;
        [SerializeField]
        Sprite[] _cellBackTextures;
        [SerializeField]
        Image _cellBack;
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

        public Point FieldPosition => new(transform.GetSiblingIndex(), transform.parent.GetSiblingIndex());

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
            transform.SetParent(this.transform);
            FitRectTransformInCell((RectTransform)transform);
        }
        #endregion
    }
}