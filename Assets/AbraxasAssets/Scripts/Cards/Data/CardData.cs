using Abraxas.StatBlocks.Data;
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
        public List<StoneWrapper> Stones;
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
                Stones = new List<StoneWrapper>(stonesCount);
                for (int i = 0; i < stonesCount; i++)
                {
                    StoneWrapper stoneWrapper = new();
                    StoneWrapper.Serialize(serializer, ref stoneWrapper);
                    Stones.Add(stoneWrapper);
                }
            }
            else
            {
                foreach (var stoneWrapper in Stones)
                {
                    StoneWrapper tempStone = stoneWrapper;
                    StoneWrapper.Serialize(serializer, ref tempStone);
                }
            }

            serializer.SerializeValue(ref StatBlock);
        }
    }
}