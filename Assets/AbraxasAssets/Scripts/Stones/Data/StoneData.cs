using System;
using Unity.Netcode;

namespace Abraxas.Stones.Data
{
    [Serializable]
    public struct StoneData : INetworkSerializable
    {
        public int Cost;
        public string Info;
        public StoneType StoneType;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Cost);
            serializer.SerializeValue(ref Info);
            serializer.SerializeValue(ref StoneType);
        }
    }
}
