using System;
using UnityEngine;

namespace Abraxas.Cards
{
    /// <summary>
    /// Card is a static class that contains settings for cards.
    /// </summary>
    public static class Card
    {
        #region Settings
        [Serializable]
        public class Settings
        {
            public GameObject cardPrefab;

            public AnimationSettings animationSettings;

            [Serializable]
            public struct AnimationSettings
            {
                public float MoveCardToFieldTime;
                public float MoveCardToDeckTime;
                public float MoveCardToGraveyardTime;
                public float MoveCardToHandTime;
                public float MoveCardFromCellToCellOnFieldTime;
                public float ScaleCardToOverlayTime;
            }
        }
        #endregion
    }
}
