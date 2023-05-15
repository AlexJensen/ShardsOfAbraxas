using System;
using UnityEngine;

namespace Abraxas.Cards
{
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
            }
        }
        #endregion
    }
}
