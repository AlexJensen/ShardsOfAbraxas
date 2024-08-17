using Abraxas.Cards.Controllers;
using System.Collections.Generic;
using Player = Abraxas.Players.Players;

namespace Abraxas.Events
{
    public class Event_PlayerDrawsCards : IEvent
    {
        #region Properties
        public Player Player { get; set; }
        public List<ICardController> CardsDrawn { get; set; }
        #endregion

        #region Methods
        public Event_PlayerDrawsCards(Player player, List<ICardController> cardsDrawn)
        {
            Player = player;
            CardsDrawn = cardsDrawn;
        }
        #endregion
    }
}
