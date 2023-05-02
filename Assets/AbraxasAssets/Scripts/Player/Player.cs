using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Abraxas.Players
{
    public enum Players
    {
        Player1,
        Player2,
        Neutral
    }

    public class Player: NetworkBehaviour
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

        [SerializeField]
        Players _player;

        public Players PlayerType
        {
            get => _player;
        }

        public void Start()
        {
            if (IsLocalPlayer)
            {
                Player[] players = FindObjectsOfType<Player>();
                if (players.Length == 1)
                {
                    _player = Players.Player1;
                }
                else
                {
                    _player = Players.Player2;
                }

                // I'll DI this when I figure out more about how it's generated in NetworkManager :/
                FindObjectOfType<PlayerManager>().RegisterLocalPlayer(_player);
            }

        }
    }
}
