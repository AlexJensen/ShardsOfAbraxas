using Abraxas.Cards;
using System.Collections;

using Player = Abraxas.Players.Players;

namespace Abraxas.Manas
{
    public interface IManaManager
    {
        int StartOfTurnManaAmount { get; }

        Mana GetPlayerMana(Player player);
        IEnumerator GenerateManaFromDeckRatio(Player player, int startOfTurnManaAmount);
        bool CanPurchaseCard(Card card);
        void IncrementStartOfTurnManaAmount();
    }
}
