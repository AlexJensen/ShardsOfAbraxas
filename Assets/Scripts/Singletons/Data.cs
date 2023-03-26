using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : Singleton<Data>
{
    [SerializeField]
    StoneData stoneData;
    [SerializeField]
    PlayerData playerData;
    public StoneData.StoneDetails GetStoneDetails(StoneData.StoneType type)
    {
        return stoneData.stones.Find(x => x.type == type);
    }

    public PlayerData.PlayerDetails GetPlayerDetails(Game.Player player)
    {
        return playerData.players.Find(x => x.player == player);
    }
}
