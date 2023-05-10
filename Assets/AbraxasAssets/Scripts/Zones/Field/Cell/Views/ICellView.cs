using Abraxas.Cards.Views;
using UnityEngine;

namespace Abraxas.Zones.Fields
{
    public interface ICellView
    {
        RectTransform RectTransform { get; }
        ICellController Controller { get; }

        void SetChild(Transform cardTransform);
    }
}
