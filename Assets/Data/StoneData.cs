using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// StoneData.cs stores top level stone information
/// </summary>
[CreateAssetMenu(fileName = "StoneData", menuName = "CardData/StoneData", order = 1)]
public class StoneData : ScriptableObject
{
    [Serializable]
    public struct StoneDetails
    {
        public string name;
        public StoneType type;
        public Color color;
    }

    public List<StoneDetails> stones;

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
