using Abraxas.Cards.Controllers;
using Abraxas.Cells.Models;
using Abraxas.Cells.Views;
using System.Drawing;
using UnityEngine;

namespace Abraxas.Cells.Controllers
{
    public interface ICellController
    {
        Players.Players Player { get; }
        Point FieldPosition { get; set; }
        int CardsOnCell { get; }
        RectTransform RectTransform { get; }

        void AddCard(ICardController card);
        void Initialize(ICellView view, ICellModel model);
        void RemoveCard(ICardController card);
        ICardController GetCardAtIndex(int v);
        void HighlightPlayableOpenCell(ICardController card);
        void SetHighlightVisible(bool v);
    }
}
