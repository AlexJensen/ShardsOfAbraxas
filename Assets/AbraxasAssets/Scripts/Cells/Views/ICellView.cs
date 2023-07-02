using Abraxas.Cells.Controllers;
using System.Drawing;
using Player = Abraxas.Players.Players;
using UnityEngine;

namespace Abraxas.Cells.Views
{
    public interface ICellView
    {
        RectTransform RectTransform { get; }
        ICellController Controller { get; }

        Player Player { get; }
        Point FieldPosition { get; }

        void Initialize(ICellController controller);
        void SetChild(Transform cardTransform);
    }
}
