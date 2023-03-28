using Abraxas.Behaviours.Game;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Abraxas.Behaviours.Data
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "CardData/PlayerData", order = 1)]
    public class PlayerData : ScriptableObject
    {
        [Serializable]
        public struct PlayerDetails
        {
            public string name;
            public GameManager.Player player;
            public Color color;
        }

        public List<PlayerDetails> players;
    }
}