using Abraxas.Players.Managers;
using Unity.Netcode;
using UnityEngine;

namespace Abraxas.Players.Views
{
    public class PlayerView: NetworkBehaviour
    {


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
                PlayerView[] players = FindObjectsOfType<PlayerView>();
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
