using Abraxas.Cards;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

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
        TOURQUOISE
    }

    public abstract class Stone : MonoBehaviour
    {
        #region Settings
        protected Settings settings;
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

        #region Dependencies
        [Inject]
        public void Construct(Settings settings)
        {
            this.settings = settings;
        }
        #endregion

        #region Fields
        [HideInInspector]
        public Card card;
        #endregion

        #region Properties
        public abstract int Cost { get;  set; }
        public abstract string Info { get; set; }
        public abstract StoneType StoneType { get; set; }
        #endregion
    }
}