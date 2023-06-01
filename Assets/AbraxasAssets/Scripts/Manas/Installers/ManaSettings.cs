using System;
using UnityEngine;

namespace Abraxas.Manas
{
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
