using System;
using UnityEngine;

namespace Abraxas.Manas
{
    /// <summary>
    /// Mana is a static class that contains serializable settings for mana.
    /// </summary>
    public static class Mana
    {
        #region Settings
        [Serializable]
        public class Settings
        {
            public GameObject ManaTypePrefab;
            public int StartingMana;
            public int ManaPerTurnIncrement;
        }
        #endregion
    }
}
