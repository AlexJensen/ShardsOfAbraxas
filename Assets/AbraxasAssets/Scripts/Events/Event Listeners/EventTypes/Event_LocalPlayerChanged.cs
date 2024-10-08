﻿using Player = Abraxas.Players.Players;

namespace Abraxas.Events
{
    public class Event_LocalPlayerChanged : IEvent
    {
        #region Properties
        public Player Player { get; set; }
        #endregion

        #region Methods
        public Event_LocalPlayerChanged(Player player)
        {
            Player = player;
        }
        #endregion
    }
}
