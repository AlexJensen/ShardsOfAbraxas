using Abraxas.Cards.Controllers;
using System.Collections;
using System.Drawing;
using Player = Abraxas.Players.Players;


namespace Abraxas.Game.Managers
{
    public interface IGameManager
    {
        IEnumerator StartGame();
        IEnumerator DrawStartOfTurnCardsForActivePlayer();
        IEnumerator GenerateStartOfTurnManaForActivePlayer();
        void RequestPurchaseCardAndMoveFromHandToCell(ICardController card, Point fieldPosition);
    }
}
