using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Abraxas.Zones.Fields;
using System.Collections;
using System.Drawing;

namespace Abraxas.Cards.Controllers
{
    public interface ICardController
    {
        ICardModelReader Model { get; }
        ICardView View { get; }
        ICardDragHandler DragHandler { get; }
        ICardMouseOverHandler MouseOverHandler { get; }
        Zones.Zones Zone { get; set; }
        bool Hidden { get; set; }
        Players.Players Owner { get; }
        Players.Players OriginalOwner { get; }
        Point FieldPosition { get; set; }
        ICellController Cell { get; set; }

        IEnumerator CheckDeath();
        IEnumerator Combat();
        IEnumerator Fight(ICardController opponent);
        IEnumerator PassHomeRow();
    }
}
