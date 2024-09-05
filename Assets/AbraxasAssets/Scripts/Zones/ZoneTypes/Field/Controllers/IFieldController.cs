using Abraxas.Cards.Controllers;
using Abraxas.Zones.Controllers;
using System.Collections;
using System.Drawing;

namespace Abraxas.Zones.Fields.Controllers
{
	public interface IFieldController : IZoneController
    {
        IEnumerator StartCombat();
        IEnumerator CombatMovement(ICardController card, Point movement);
        IEnumerator MoveCardToCell(ICardController card, Point fieldPos);
        PointF GetDefaultCellDimensions();
        void HighlightPlayableOpenCells(ICardController cardController);
        void SetHighlightVisible(bool val);
    }
}
