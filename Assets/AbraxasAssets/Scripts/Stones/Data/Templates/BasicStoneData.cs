using System;
using Unity.Netcode;
using UnityEngine;

namespace Abraxas.Stones.Data
{
    [Serializable]
    public struct BasicStoneData : IStoneData
    {
        [SerializeField]
        private int cost;
        [SerializeField]
        private string info;
        [SerializeField]
        private StoneType stoneType;

        public int Cost { readonly get => cost; set => cost = value; }
        public string Info { readonly get => info; set => info = value; }
        public StoneType StoneType { readonly get => stoneType; set => stoneType = value; }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            var cost = Cost;
            var info = Info;
            serializer.SerializeValue(ref cost);
            serializer.SerializeValue(ref info);
            Cost = cost;
            Info = info;
            int stoneTypeInt = (int)StoneType;
            serializer.SerializeValue(ref stoneTypeInt);
            if (serializer.IsReader)
            {
                StoneType = (StoneType)stoneTypeInt;
            }
        }
    }
}
