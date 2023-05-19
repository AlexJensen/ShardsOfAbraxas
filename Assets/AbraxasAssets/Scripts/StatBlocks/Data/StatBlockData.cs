using Abraxas.Stones;
using System;
using UnityEngine;

namespace Abraxas.StatBlocks.Data
{
    [Serializable]
    public struct StatBlockData
    {
        #region Fields
        Vector3Int _stats;
        #endregion

        #region Properties
        public int Cost { get; internal set; }
        public StoneType StoneType { get; internal set; }

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
