using Abraxas.Cards.Controllers;
using Abraxas.Zones.Controllers;
using Abraxas.Zones.Fields.Views;
using System.Collections;
using System.Drawing;

namespace Abraxas.Zones.Fields.Controllers
{
    public interface IFieldController : IZoneController
    {
        void AddCard(ICardController card, Point fieldPos);
        IEnumerator StartCombat();
        IEnumerator MoveCardAndFight(ICardController card, Point movement);
        IEnumerator MoveCardToCell(ICardController card, Point fieldPos);
        PointF GetDefaultCellDimensions();
    }
}
