using Abraxas.Players.Managers;
using Unity.Netcode;
using UnityEngine;

namespace Abraxas.Players
{
    /// <summary>
    /// PlayerInstance is a NetworkBehaviour that represents a player in the game.
    /// </summary>
    public class PlayerInstance : NetworkBehaviour
    {


        [SerializeField]
        Players _player;

        public Players PlayerType
        {
            get => _player;
        }

        public void Start()
        {
            //TODO: This approach to player assignment won't work with rejoining players.
            if (IsLocalPlayer)
            {
                PlayerInstance[] players = FindObjectsOfType<PlayerInstance>();
                if (players.Length == 1)
                {
                    _player = Players.Player1;
                }
                else
                {
                    _player = Players.Player2;
                }

                // I'll DI this when I figure out more about how it's generated in NetworkManager :/
                StartCoroutine(FindObjectOfType<PlayerManager>().RegisterLocalPlayer(_player));
            }

        }
    }
}
