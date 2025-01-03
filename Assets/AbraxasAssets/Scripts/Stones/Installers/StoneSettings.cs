﻿using System;
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
        TURQUOISE,
        INVALID
    }

    public static class Stone
    {
        #region Settings
        [Serializable]
        public class Settings
        {
            [Serializable]
            public struct StoneTypeDetails
            {
                public string name;
                public StoneType type;
                public Color color;
                public Material material;
            }

            public List<StoneTypeDetails> stones;


            public virtual StoneTypeDetails GetStoneTypeDetails(StoneType type)
            {
                return stones.Find(t => t.type == type);
            }
        }
        #endregion
    }
}
