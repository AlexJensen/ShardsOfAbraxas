using Abraxas.Core;
using System;
using System.Collections.Generic;
using Unity.Netcode;

namespace Abraxas.Stones.Data
{
    [Serializable]
    public class StoneConnector
    {

        public StoneSO RuntimeStoneData;

        public List<int> ConnectionIndexes;

        public StoneConnector() { }

        public StoneConnector(StoneSO stoneData)
        {
            RuntimeStoneData = stoneData;
            if (stoneData is TriggerStoneSO triggerStoneData)
            {
                triggerStoneData.Indexes = ConnectionIndexes;
            }
        }


        public int Index { get; internal set; }

        public static void Serialize<T>(BufferSerializer<T> serializer, ref StoneConnector connector) where T : IReaderWriter

        {
            string typeId = string.Empty;
            if (!serializer.IsReader)
            {
                typeId = connector.RuntimeStoneData.Data.GetType().AssemblyQualifiedName;
            }
            serializer.SerializeValue(ref typeId);

            if (serializer.IsReader)
            {
                var instance = Utilities.CreateInstanceFromStoneCache<StoneSO>(typeId);
                if (instance != null)
                {
                    connector = new StoneConnector(instance);
                    instance.Data.NetworkSerialize(serializer);
                }
            }
            else
            {
                connector.RuntimeStoneData.Data.NetworkSerialize(serializer);
            }
        }
    }
}