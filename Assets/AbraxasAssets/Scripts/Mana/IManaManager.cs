using Abraxas.Cards;
using System.Collections;

using Player = Abraxas.Players.Players;

namespace Abraxas.Manas
{
    public interface IManaManager
    {
        int StartOfTurnMana { get; }

        IEnumerator GenerateManaFromDeckRatio(Player player, int startOfTurnMana);
        bool CanPurchaseCard(Card card);
        void IncrementStartOfTurnManaAmount();
        void PurchaseCard(Card card);
    }
}
