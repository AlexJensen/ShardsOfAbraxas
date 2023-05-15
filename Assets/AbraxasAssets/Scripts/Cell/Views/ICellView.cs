using Abraxas.Cells.Controllers;
using UnityEngine;

namespace Abraxas.Cells.Views
{
    public interface ICellView
    {
        RectTransform RectTransform { get; }
        ICellController Controller { get; }

        void SetChild(Transform cardTransform);
    }
}
