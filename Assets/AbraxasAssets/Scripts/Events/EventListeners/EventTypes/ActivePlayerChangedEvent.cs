using Player = Abraxas.Players.Players;

namespace Abraxas.Events
{
    public class ActivePlayerChangedEvent : IEvent
    {
        #region Properties
        public Player Player { get; }
        #endregion

        #region Methods
        public ActivePlayerChangedEvent(Player player)
        {
            Player = player;
        }
        #endregion
    }
}
