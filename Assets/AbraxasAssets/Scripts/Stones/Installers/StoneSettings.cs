using System;
using System.Collections.Generic;
using UnityEngine;

namespace Abraxas.Stones
{
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
        TURQUOISE
    }

    public static class Stone
    {
        #region Settings
        [Serializable]
        public class Settings
        {
            [Serializable]
            public struct StoneDetails
            {
                public string name;
                public StoneType type;
                public Color color;
            }

            public List<StoneDetails> stones;

            public StoneDetails GetStoneDetails(StoneType type)
            {
                return stones.Find(t => t.type == type);
            }
        }
        #endregion
    }
}
