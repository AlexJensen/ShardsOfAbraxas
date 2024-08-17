using Abraxas.Cells.Controllers;
using Abraxas.Manas;
using Abraxas.Stones;
using Abraxas.UI;
using System.Collections;
using System.Collections.Generic;
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
        void ChangeScale(PointF pointF, float time);
        void SetCardPositionToMousePosition();
        string GetCostText();
        IEnumerator MoveToCell(ICellController cell, float moveCardTime);
        void UpdateCostText(string totalCost);
        void SetHighlight(bool isPlayable);
        void UpdateCostTextWithManaTypes(List<ManaType> manaTypes, Dictionary<StoneType, int> totalCosts, bool isPlayable, bool isInHand);
        void SetToInitialScale();
    }
}
