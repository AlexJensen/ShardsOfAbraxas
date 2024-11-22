using Abraxas.StatBlocks.Data;
using Abraxas.Stones.Conditions;
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

            // Serialize the core stone data
            stone.NetworkSerialize(serializer);

            // If the stone has conditions, serialize them
            if (stone is TriggerStoneSO triggerStone)
            {
                int conditionCount = triggerStone.Conditions?.Count ?? 0;
                serializer.SerializeValue(ref conditionCount);

                for (int i = 0; i < conditionCount; i++)
                {
                    var condition = triggerStone.Conditions[i] as ConditionSO;
                    if (condition != null)
                    {
                        string conditionTypeId = condition.GetType().AssemblyQualifiedName;
                        serializer.SerializeValue(ref conditionTypeId);

                        condition.NetworkSerialize(serializer);
                    }
                }
            }
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
                    // Deserialize the core stone data
                    stone.NetworkSerialize(serializer);

                    // If the stone is a TriggerStone, deserialize the conditions
                    if (stone is TriggerStoneSO triggerStone)
                    {
                        int conditionCount = 0;
                        serializer.SerializeValue(ref conditionCount);

                        triggerStone.Conditions = new List<ScriptableObject>(conditionCount);
                        for (int i = 0; i < conditionCount; i++)
                        {
                            string conditionTypeId = string.Empty;
                            serializer.SerializeValue(ref conditionTypeId);

                            var conditionType = Type.GetType(conditionTypeId);
                            if (conditionType != null)
                            {
                                var condition = ScriptableObject.CreateInstance(conditionType) as ConditionSO;
                                if (condition != null)
                                {
                                    condition.NetworkSerialize(serializer);
                                    triggerStone.Conditions.Add(condition);
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}
