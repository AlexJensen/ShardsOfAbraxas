using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "CardData/PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    [Serializable]
    public struct PlayerDetails
    {
        public string name;
        public Game.Player player;
        public Color color;
    }

    public List<PlayerDetails> players;
}
