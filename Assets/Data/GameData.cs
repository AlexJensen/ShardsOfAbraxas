using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "StoneColors", menuName = "CardData/StoneColors", order = 1)]
public class GameData : ScriptableObject
{
    [Serializable]
    public struct StoneColor
    {
        public string name;
        public StoneTypes type;
        public Color color;
    }

    public List<StoneColor> stoneColors;

    public enum Players
    {
        PLAYER_1,
        PLAYER_2,
        NEUTRAL
    }

    public enum StoneTypes
    {
        GARNET,
        AMETHYST,
        AQUAMARINE,
        DIAMOND,
        EMERALD,
        MOONSTONE,
        RUBY,
        PERIDOT,
        SAPPHIRE,
        TOURMALINE,
        CITRINE,
        TOURQUOISE
    }
}
