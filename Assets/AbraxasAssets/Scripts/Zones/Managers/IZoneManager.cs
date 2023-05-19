using Abraxas.Cards.Controllers;
using System.Collections;
using System.Drawing;

namespace Abraxas.Zones.Managers
{
    public interface IZoneManager
    {
        #region Methods
        IEnumerator MoveCardFromFieldToDeck(ICardController card);
        IEnumerator MoveCardFromFieldToGraveyard(ICardController card);
        IEnumerator MoveCardFromHandToCell(ICardController card, Point fieldPosition);
        IEnumerator MoveCardsFromDeckToGraveyard(Players.Players player, int amount, int index = 0);
        IEnumerator MoveCardsFromDeckToHand(Players.Players player, int amount, int index = 0);
        IEnumerator ShuffleDeck(Players.Players player1);
        void BuildDecks();
        #endregion
    }
}
