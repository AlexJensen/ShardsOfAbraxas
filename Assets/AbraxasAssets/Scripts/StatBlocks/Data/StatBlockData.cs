using Abraxas.Stones;
using System;
using Unity.Netcode;
using UnityEngine;

namespace Abraxas.StatBlocks.Data
{
    [Serializable]
    public struct StatBlockData : INetworkSerializable
    {
        #region Fields
        public Vector3Int Stats;
        public int Cost;
        public StoneType StoneType;
        #endregion

        #region Properties
        public int this[StatValues index]
        {
            get => Stats[(int)index];
            set
            {
                Stats[(int)index] = value;
            }
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Stats);
            serializer.SerializeValue(ref Cost);
            serializer.SerializeValue(ref StoneType);
        }
        #endregion
    }
}
