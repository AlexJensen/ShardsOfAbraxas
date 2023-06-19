using Abraxas.Cells.Controllers;
using System.Drawing;
using UnityEngine;
using Zenject;
using Image = UnityEngine.UI.Image;
using Player = Abraxas.Players.Players;
namespace Abraxas.Cells.Views
{
    class CellView : MonoBehaviour, ICellView
    {
        #region Dependencies
        ICellController _controller;
        Cell.Settings _cellSettings;
        [Inject]
        public void Construct(Cell.Settings cellSettings)
        {
            _cellSettings = cellSettings;
        }

        public void Initialize(ICellController controller)
        {
            _controller = controller;

            _cellBack.sprite = _cellSettings.cellBackTextures[Random.Range(0, _cellSettings.cellBackTextures.Length)];
        }
        #endregion

        #region Fields
        [SerializeField]
        Player _player;
        [SerializeField]
        Image _cellBack;
        [SerializeField]
        RectTransform _cardHolder;
        #endregion

        #region Properties
        public ICellController Controller => _controller;
        public RectTransform RectTransform { get => _cardHolder; }

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
            transform.SetParent(RectTransform);
            FitRectTransformInCell(RectTransform);
        }
        #endregion
    }
}