using Abraxas.StatBlocks.Data;
using System;
using Unity.Netcode;
using UnityEngine;

namespace Abraxas.Stones.Data
{
    [Serializable]
    public struct ModifyCardStatBlockData : IStoneData
    {
        [SerializeField]
        private int cost;
        [SerializeField]
        private string info;
        [SerializeField]
        private StoneType stoneType;
        [SerializeField]
        private StatData modification;

        public int Cost { readonly get => cost; set => cost = value; }
        public string Info { readonly get => info; set => info = value; }
        public StoneType StoneType { readonly get => stoneType; set => stoneType = value; }
        public StatData Modification { readonly get => modification; set => modification = value; }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            var cost = Cost;
            var info = Info;
            var modification = Modification;
            serializer.SerializeValue(ref cost);
            serializer.SerializeValue(ref info);
            serializer.SerializeValue(ref modification);
            Cost = cost;
            Info = info;
            Modification = modification;
            int stoneTypeInt = (int)(StoneType);
            serializer.SerializeValue(ref stoneTypeInt);
            if (serializer.IsReader)
            {
                StoneType = (StoneType)stoneTypeInt;
            }
        }
    }
}
