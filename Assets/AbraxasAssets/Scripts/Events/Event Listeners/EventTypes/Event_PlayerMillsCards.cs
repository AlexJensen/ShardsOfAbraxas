using Player = Abraxas.Players.Players;

namespace Abraxas.Events
{
    public class Event_PlayerMillsCards : IEvent
    {
        #region Properties
        public Player Player { get; set; }
        public int Amount { get; set; }
        #endregion

        #region Methods
        public Event_PlayerMillsCards(Player player, int amount)
        {
            Player = player;
            Amount = amount;
        }
        #endregion
    }
}
