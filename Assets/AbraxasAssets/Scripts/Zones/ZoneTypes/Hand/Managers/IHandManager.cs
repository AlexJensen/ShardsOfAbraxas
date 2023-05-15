using Abraxas.Cards.Controllers;
using System.Collections;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Hands.Managers
{
    public interface IHandManager
    {
        IEnumerator MoveCardToHand(Player player, ICardController card);
        void RemoveCard(Player player, ICardController card);
        IEnumerator ReturnCardToHand(ICardController card);
    }
}
