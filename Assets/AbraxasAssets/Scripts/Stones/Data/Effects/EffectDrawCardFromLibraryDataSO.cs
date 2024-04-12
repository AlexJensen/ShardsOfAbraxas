using Abraxas.Stones.Controllers;
using Abraxas.Stones.Data;
using System;
using Unity.Netcode;
using UnityEngine;

namespace Abraxas.Stones.Types
{
    [CreateAssetMenu(fileName = "New Effect DrawCardFromLibrary", menuName = "Abraxas/StoneData/Effects/Draw Card From Library")]
    [Serializable]
    public class EffectDrawCardFromLibraryDataSO : EffectStoneDataSO
    {
        [SerializeField]
        Effect_DrawCardFromLibraryData _data = new()
        {
            Info = "Draw a card."
        };
        public override IStoneData Data { get => _data; set => _data = (Effect_DrawCardFromLibraryData)value; }
        public override Type ControllerType { get; set; } = typeof(Effect_DrawCardFromLibrary);
    }

    [Serializable]
    public struct Effect_DrawCardFromLibraryData : IStoneData
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
