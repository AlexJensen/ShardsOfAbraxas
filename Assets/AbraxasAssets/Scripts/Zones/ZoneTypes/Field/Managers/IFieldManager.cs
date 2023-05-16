using Abraxas.Cards.Controllers;
using System.Collections;
using System.Drawing;


namespace Abraxas.Zones.Fields.Managers
{
    public interface IFieldManager
    {
        void RemoveCard(ICardController card);
        IEnumerator MoveCardToCell(ICardController card, Point fieldPosition);
        void AddCard(ICardController card, Point fieldPosition);
        PointF GetCellDimensions();
        IEnumerator StartCombat();
        IEnumerator MoveCardAndFight(ICardController card, Point movement);
    }
}
