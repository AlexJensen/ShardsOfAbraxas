using Abraxas.Cards.Controllers;
using Abraxas.Core;
using System.Collections;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Hands.Managers
{
    public interface IHandManager : IManager
    {
        ICardController CardDragging { get; set; }
        IEnumerator MoveCardToHand(Player player, ICardController card);
        void RemoveCard(ICardController card);
        IEnumerator ReturnCardToHand(ICardController card);
    }
}
