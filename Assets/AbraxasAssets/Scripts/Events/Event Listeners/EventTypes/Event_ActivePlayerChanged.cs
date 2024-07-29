using Player = Abraxas.Players.Players;

namespace Abraxas.Events
{
    public class Event_ActivePlayerChanged : IEvent<Player>
    {
        #region Properties
        public Player Data { get; set; }
        #endregion

        #region Methods
        public Event_ActivePlayerChanged(Player player)
        {
            Data = player;
        }
        #endregion
    }
}
