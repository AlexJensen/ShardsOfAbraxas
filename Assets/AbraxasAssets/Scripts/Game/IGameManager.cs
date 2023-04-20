using Abraxas.Cards;
using Abraxas.Zones.Fields;
using System.Collections;
using UnityEngine;
using Player = Abraxas.Players.Players;


namespace Abraxas.Game
{
    public interface IGameManager
    {
        Player ActivePlayer { get; }
        IEnumerator StartGame();
        IEnumerator BeginNextGameState();
        IEnumerator DrawStartOfTurnCardsForActivePlayer();
        IEnumerator GenerateStartOfTurnManaForActivePlayer();
        void ToggleActivePlayer();
        void ModifyPlayerHealth(Player players, int amount);
        IEnumerator ShowCardDetail(Card card);
        IEnumerator HideCardDetail();
        IEnumerator MoveCardFromFieldToDeck(Card card);
        IEnumerator MoveCardFromFieldToGraveyard(Card card);
        IEnumerator MoveCardsFromDeckToGraveyard(Player player, int amount, int index = 0);
        IEnumerator MoveCardsFromDeckToHand(Player player, int amount, int index = 0);
        IEnumerator MoveCardFromHandToCell(Card card, Vector2Int cell);
        IEnumerator MoveCardAndFight(Card card, Vector2Int vector2Int);
        IEnumerator MoveCardToHand(Card card);
        void PurchaseCard(Card card);
        bool CanPurchaseCard(Card card);
        object MoveCardFromCellToCell(Cell cell1, Cell cell2);
    }
}
