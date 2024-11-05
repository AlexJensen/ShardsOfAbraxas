using Abraxas.StatBlocks.Data;
using Abraxas.Stones.Data;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Player = Abraxas.Players.Players;

namespace Abraxas.Cards.Data
{

    /// <summary>
    /// CardData is a serializable struct that contains all data needed to create a card.
    /// </summary>
    [Serializable]
    public struct CardData : INetworkSerializable
    {
        public string Title;
        [HideInInspector]
        public Player Owner;
        [HideInInspector]
        public Player OriginalOwner;
        public List<StoneSO> Stones;
        public StatBlockData StatBlock;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Title);
            serializer.SerializeValue(ref Owner);
            serializer.SerializeValue(ref OriginalOwner);

            int stonesCount = Stones != null ? Stones.Count : 0;
            serializer.SerializeValue(ref stonesCount);

            if (serializer.IsReader)
            {
                Stones = new List<StoneSO>(stonesCount);
                for (int i = 0; i < stonesCount; i++)
                {
                    StoneSO stone = null;
                    SerializeStone(serializer, ref stone);
                    Stones.Add(stone);
                }
            }
            else
            {
                foreach (var stone in Stones)
                {
                    SerializeStone(serializer, stone);
                }
            }

            serializer.SerializeValue(ref StatBlock);
        }

        private readonly void SerializeStone<T>(BufferSerializer<T> serializer, StoneSO stone) where T : IReaderWriter
        {
            string typeId = stone.GetType().AssemblyQualifiedName;
            serializer.SerializeValue(ref typeId);

            stone.NetworkSerialize(serializer);
        }

        private readonly void SerializeStone<T>(BufferSerializer<T> serializer, ref StoneSO stone) where T : IReaderWriter
        {
            string typeId = string.Empty;
            serializer.SerializeValue(ref typeId);

            var stoneType = Type.GetType(typeId);
            if (stoneType != null)
            {
                stone = ScriptableObject.CreateInstance(stoneType) as StoneSO;
                if (stone != null)
                {
                    stone.NetworkSerialize(serializer);
                }
            }
        }
    }
}
