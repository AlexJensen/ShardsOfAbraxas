using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// GameData.cs stores top level game data and enums for commonly used terms
/// </summary>
[CreateAssetMenu(fileName = "StoneColors", menuName = "CardData/StoneColors", order = 1)]
public class StoneData : ScriptableObject
{
    [Serializable]
    public struct StoneColor
    {
        public string name;
        public StoneType type;
        public Color color;
    }

    public List<StoneColor> stoneColors;

    public enum StoneType
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
