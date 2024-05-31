
using Unity.Netcode;

namespace Abraxas.Stones.Data
{
    public interface IStoneData
    {
        int Cost { get; set; }
        string Info { get; set; }
        StoneType StoneType { get; set; }
        void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter;
    }
}
