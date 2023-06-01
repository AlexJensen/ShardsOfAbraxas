using Abraxas.Cards.Controllers;
using System.Collections;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Hands.Managers
{
    public interface IHandManager
    {
        ICardController CardDragging { get; set; }
        IEnumerator MoveCardToHand(Player player, ICardController card);
        void RemoveCard(ICardController card);
        IEnumerator ReturnCardToHand(ICardController card);
    }
}
