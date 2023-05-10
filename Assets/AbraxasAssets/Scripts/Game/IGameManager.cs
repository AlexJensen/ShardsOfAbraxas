using Abraxas.Cards.Controllers;
using System.Collections;
using System.Drawing;
using UnityEngine;
using Player = Abraxas.Players.Players;


namespace Abraxas.Game
{
    public interface IGameManager
    {
        IEnumerator StartGame();
        IEnumerator DrawStartOfTurnCardsForActivePlayer();
        IEnumerator GenerateStartOfTurnManaForActivePlayer();
        IEnumerator MoveCardFromFieldToDeck(ICardController card);
        IEnumerator MoveCardFromFieldToGraveyard(ICardController card);
        IEnumerator MoveCardsFromDeckToGraveyard(Player player, int amount, int index = 0);
        IEnumerator MoveCardsFromDeckToHand(Player player, int amount, int index = 0);
        IEnumerator MoveCardFromHandToCell(ICardController card, Point cell);
        void RequestPurchaseCardAndMoveFromHandToCell(ICardController card, Point fieldPosition);
    }
}
