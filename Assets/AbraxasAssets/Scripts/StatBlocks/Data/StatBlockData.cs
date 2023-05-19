using Abraxas.Stones;
using System;
using UnityEngine;

namespace Abraxas.StatBlocks.Data
{
    [Serializable]
    public struct StatBlockData
    {
        #region Fields
        public Vector3Int _stats;
        public int Cost;
        public StoneType StoneType;
        #endregion

        #region Properties
        public int this[StatValues index]
        {
            get => _stats[(int)index];
            set
            {
                _stats[(int)index] = value;
            }
        }
        #endregion
    }
}
