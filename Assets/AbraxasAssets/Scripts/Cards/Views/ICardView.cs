using Abraxas.Cells.Controllers;
using Abraxas.Events;
using System.Collections;
using System.Drawing;
using Unity.Netcode;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace Abraxas.Cards.Views
{
    public interface ICardView
    {
        Image Image { get; }
        Transform Transform { get; }
        NetworkObject NetworkObject { get; }
        RectTransformMover RectTransformMover { get; }

        void UpdateCostTextWithCastability(ManaModifiedEvent eventData);
        void ChangeScale(PointF pointF, float time);
        void SetCardPositionToMousePosition();
        string GetCostText();
        IEnumerator MoveToCell(ICellController cell, float moveCardTime);
    }
}
