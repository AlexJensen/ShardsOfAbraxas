using Abraxas.Cards.Controllers;
using System.Collections;
using System.Drawing;
using Player = Abraxas.Players.Players;


namespace Abraxas.Games.Managers
{
    public interface IGameManager
    {
        IEnumerator StartGame();
        IEnumerator DrawStartOfTurnCardsForActivePlayer();
        IEnumerator GenerateStartOfTurnManaForActivePlayer();
        void RequestPurchaseCardAndMoveFromHandToCell(ICardController card, Point fieldPosition);

        IEnumerator DrawCard(Player player, int amount = 1, int index = 0);
        IEnumerator MillCard(Player player, int amount = 1, int index = 0);
        bool IsAnyPlayerInputAvailable();
        IEnumerator EndGame(Player player);
    }
}
