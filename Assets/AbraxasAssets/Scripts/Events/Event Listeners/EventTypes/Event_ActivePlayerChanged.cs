using Player = Abraxas.Players.Players;

namespace Abraxas.Events
{
    public class Event_ActivePlayerChanged : IEvent
    {
        #region Properties
        public Player Player { get; set; }
        #endregion

        #region Methods
        public Event_ActivePlayerChanged(Player player)
        {
            Player = player;
        }
        #endregion
    }
}
