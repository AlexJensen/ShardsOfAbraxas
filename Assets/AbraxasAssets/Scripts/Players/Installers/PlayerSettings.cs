using System;
using System.Collections.Generic;
using UnityEngine;

namespace Abraxas.Players
{
    [Serializable]
    public enum Players
    {
        Player1,
        Player2,
        Neutral
    }
    public static class Player
    {
        #region Settings
        [Serializable]
        public class Settings
        {
            public int StartingHealth;
            [Serializable]
            public struct PlayerDetails
            {
                public string name;
                public Players player;
                public Color color;
            }

            public List<PlayerDetails> players;

            public PlayerDetails GetPlayerDetails(Players player)
            {
                return players.Find(x => x.player == player);
            }
        }
        #endregion
    }
}
