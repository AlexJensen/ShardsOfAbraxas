using Abraxas.Cards.Controllers;
using Abraxas.Zones.Fields.Controllers;
using System.Collections;
using System.Drawing;


namespace Abraxas.Zones.Fields.Managers
{
    public interface IFieldManager
    {
        void RemoveCard(ICardController card);
        IEnumerator MoveCardToCell(ICardController card, Point fieldPosition);
        PointF GetCellDimensions();
        IEnumerator StartCombat();
        IEnumerator CombatMovement(ICardController card, Point movement);
        void SetField(IFieldController fieldController);
        void AddCard(ICardController card);
    }
}
