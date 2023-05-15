using Abraxas.Cards.Controllers;
using Abraxas.Cards.Models;
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
        ICardModelReader Model { get; }
        Image Image { get; }
        Transform Transform { get; }
        NetworkBehaviourReference NetworkBehaviourReference { get; }
        RectTransformMover RectTransformMover { get; }

        void UpdateCostTextWithCastability(ManaModifiedEvent eventData);
        void ChangeScale(PointF pointF, float time);
        void SetCardPositionToMousePosition();
        string GetCostText();
        IEnumerator MoveToCell(ICellView cell, float moveCardTime);
    }
}
