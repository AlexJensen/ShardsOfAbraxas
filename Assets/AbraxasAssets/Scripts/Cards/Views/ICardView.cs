using Abraxas.Cards.Controllers;
using Abraxas.Cells.Views;
using Abraxas.Events;
using System.Collections;
using System.Drawing;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Abraxas.Cards.Views
{
    public interface ICardView
    {
        ICardController Controller { get; }
        Image Image { get; }
        Transform Transform { get; }
        NetworkObject NetworkObject { get; }
        RectTransformMover RectTransformMover { get; }

        void UpdateCostTextWithCastability(ManaModifiedEvent eventData);
        void ChangeScale(PointF pointF, float time);
        void SetCardPositionToMousePosition();
        string GetCostText();
        IEnumerator MoveToCell(ICellView cell, float moveCardTime);
    }
}
