using Abraxas.Stones;
using System.Collections.Generic;
using Unity.Netcode;

namespace Abraxas.Manas.Models
{


    [System.Serializable]
    public struct ManaAmounts : INetworkSerializable
    {
        public List<StoneType> ManaTypes;
        public List<int> Amounts;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            int count = ManaTypes != null ? ManaTypes.Count : 0;
            serializer.SerializeValue(ref count);

            if (serializer.IsReader)
            {
                ManaTypes = new List<StoneType>(count);
                Amounts = new List<int>(count);
            }

            for (int i = 0; i < count; i++)
            {
                StoneType manaType = StoneType.INVALID;
                int amount = 0;

                if (!serializer.IsReader)
                {
                    manaType = ManaTypes[i];
                    amount = Amounts[i];
                }

                serializer.SerializeValue(ref manaType);
                serializer.SerializeValue(ref amount);

                if (serializer.IsReader)
                {
                    ManaTypes.Add(manaType);
                    Amounts.Add(amount);
                }
            }
        }
    }

}
