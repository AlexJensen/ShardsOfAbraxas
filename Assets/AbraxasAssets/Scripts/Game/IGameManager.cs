using Abraxas.Cards;
using Abraxas.Stones;
using Abraxas.Zones.Fields;
using System.Collections;
using System.Collections.Generic;
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
        IEnumerator MoveCardAndFight(Card card, Vector2Int vector2Int);
        object MoveCardFromCellToCell(Cell cell1, Cell cell2);
        Dictionary<StoneType, int> GetDeckCost(Player player);
    }
}
