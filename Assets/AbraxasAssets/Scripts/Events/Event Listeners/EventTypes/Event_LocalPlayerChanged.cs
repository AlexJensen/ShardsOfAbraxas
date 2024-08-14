using Player = Abraxas.Players.Players;

namespace Abraxas.Events
{
    public class Event_LocalPlayerChanged : IEvent<Player>
    {
        #region Properties
        public Player Data { get; set; }
        #endregion

        #region Methods
        public Event_LocalPlayerChanged(Player player)
        {
            Data = player;
        }
        #endregion
    }
}
