using Abraxas.Behaviours.Game;
using Abraxas.Behaviours.Players;
using Abraxas.Core;
using UnityEngine;

namespace Abraxas.Behaviours.Data
{
    public class DataManager : MonoBehaviour
    {
        [SerializeField]
        StoneData _stoneData;
        [SerializeField]
        PlayerData _playerData;
        public StoneData.StoneDetails GetStoneDetails(StoneData.StoneType type)
        {
            return _stoneData.stones.Find(x => x.type == type);
        }

        public PlayerData.PlayerDetails GetPlayerDetails(Player player)
        {
            return _playerData.players.Find(x => x.player == player);
        }
    }
}
