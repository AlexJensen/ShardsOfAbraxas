using Abraxas.Cards.Controllers;
using System.Collections;
using System.Drawing;
using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Managers
{
    public interface IZoneManager
    {
        #region Methods
        IEnumerator MoveCardFromFieldToDeck(ICardController card, Player player);
        IEnumerator MoveCardFromFieldToGraveyard(ICardController card, Player player);
        IEnumerator MoveCardFromHandToCell(ICardController card, Point fieldPosition);
        IEnumerator MoveCardFromDeckToGraveyard(ICardController card, Player player);
        IEnumerator MoveCardFromDeckToHand(ICardController card, Player player);
        #endregion
    }
}
