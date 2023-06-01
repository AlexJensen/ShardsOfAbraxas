using System;

namespace Abraxas.Hands
{
    public static class Hand
    {
        #region Settings
        [Serializable]
        public class Settings
        {
            [Serializable]
            public struct PlaceholderSettings
            {
                public float minScale;
                public float maxScale;
                public float scaleToMaxSizeTime;
            }
            public PlaceholderSettings placeholderSettings;
        }
        #endregion
    }
}
