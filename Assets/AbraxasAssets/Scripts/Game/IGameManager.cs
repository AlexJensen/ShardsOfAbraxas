using Abraxas.Cards;
using System.Collections;
using UnityEngine;
using Player = Abraxas.Players.Players;


namespace Abraxas.Game
{
    public interface IGameManager
    {
        IEnumerator StartGame();
        IEnumerator DrawStartOfTurnCardsForActivePlayer();
        IEnumerator GenerateStartOfTurnManaForActivePlayer();
        IEnumerator MoveCardFromFieldToDeck(Card card);
        IEnumerator MoveCardFromFieldToGraveyard(Card card);
        IEnumerator MoveCardsFromDeckToGraveyard(Player player, int amount, int index = 0);
        IEnumerator MoveCardsFromDeckToHand(Player player, int amount, int index = 0);
        IEnumerator MoveCardFromHandToCell(Card card, Vector2Int cell);
        void PurchaseCardAndMoveFromHandToCell(Card card, Vector2Int fieldPosition);
    }
}
