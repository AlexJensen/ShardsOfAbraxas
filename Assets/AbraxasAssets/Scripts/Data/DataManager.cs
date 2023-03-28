using Abraxas.Behaviours.Game;
using Abraxas.Core;
using UnityEngine;

namespace Abraxas.Behaviours.Data
{
    public class DataManager : Singleton<DataManager>
    {
        [SerializeField]
        StoneData stoneData;
        [SerializeField]
        PlayerData playerData;
        public StoneData.StoneDetails GetStoneDetails(StoneData.StoneType type)
        {
            return stoneData.stones.Find(x => x.type == type);
        }

        public PlayerData.PlayerDetails GetPlayerDetails(GameManager.Player player)
        {
            return playerData.players.Find(x => x.player == player);
        }
    }
}
