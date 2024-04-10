using Abraxas.Core;
using Abraxas.Stones.Data;
using System;
using Unity.Netcode;

[Serializable]
public class StoneWrapper
{
	public StoneDataSO RuntimeStoneData;

	public StoneWrapper() { }

	public StoneWrapper(StoneDataSO stoneData)
	{
		RuntimeStoneData = stoneData;
	}

	public static void Serialize<T>(BufferSerializer<T> serializer, ref StoneWrapper wrapper) where T : IReaderWriter
	{
		string typeId = string.Empty;
		if (!serializer.IsReader)
		{
			typeId = wrapper.RuntimeStoneData.Data.GetType().AssemblyQualifiedName;
		}
		serializer.SerializeValue(ref typeId);

		if (serializer.IsReader)
		{
			var instance = Utilities.CreateInstance<StoneDataSO>(typeId);
			if (instance != null)
			{
				wrapper = new StoneWrapper(instance);
				instance.Data.NetworkSerialize(serializer);
			}
		}
		else
		{
			wrapper.RuntimeStoneData.Data.NetworkSerialize(serializer);
		}
	}
}