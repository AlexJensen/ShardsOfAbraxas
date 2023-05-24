using Abraxas.StatBlocks.Data;
using Abraxas.Stones.Data;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using Player = Abraxas.Players.Players;

namespace Abraxas.Cards.Data
{
    [Serializable]
    public struct CardData : INetworkSerializable
    {
        #region Fields
        public string Title;
        public Player Owner;
        public Player OriginalOwner;
        public List<StoneData> Stones;
        public StatBlockData StatBlock;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Title);
            serializer.SerializeValue(ref Owner);
            serializer.SerializeValue(ref OriginalOwner);
            StoneData[] stoneArray = Stones?.ToArray();
            serializer.SerializeValue(ref stoneArray);
            serializer.SerializeValue(ref StatBlock);
        }
        #endregion
    }
}
