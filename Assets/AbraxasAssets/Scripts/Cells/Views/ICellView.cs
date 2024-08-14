using Abraxas.Cells.Controllers;
using System.Drawing;
using UnityEngine;
using Player = Abraxas.Players.Players;

namespace Abraxas.Cells.Views
{
    public interface ICellView
    {
        RectTransform RectTransform { get; }
        ICellController Controller { get; }

        Player Player { get; }

        void Initialize(ICellController controller);
        void SetChild(Transform cardTransform);
        void UpdateText(string Text);
    }
}
