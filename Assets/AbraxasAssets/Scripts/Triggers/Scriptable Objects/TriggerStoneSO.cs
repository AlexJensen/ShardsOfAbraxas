using Abraxas.Stones.Conditions;
using Abraxas.Stones.EventListeners;
using Abraxas.Stones.Triggers;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace Abraxas.Stones.Data
{
    [CreateAssetMenu(fileName = "New Trigger", menuName = "Abraxas/Data/StoneData/Trigger")]
    public class TriggerStoneSO : StoneSO
    {
        #region Fields
        public List<int> ConnectionIndexes = new();

        [SerializeField]
        List<ScriptableObject> _conditions = new();

        [SerializeField]
        List<ScriptableObject> _eventListeners = new();  // Use the base class here

        [SerializeField]
        BasicStoneData _data = new();
        #endregion

        #region Properties
        public List<ScriptableObject> Conditions { get => _conditions; set => _conditions = value; }
        public List<ScriptableObject> EventListeners { get => _eventListeners; set => _eventListeners = value; }
        public override Type ControllerType { get; set; } = typeof(TriggerStone);
        public override IStoneData Data { get => _data; set => _data = (BasicStoneData)value; }
        #endregion

        #region Serialization
        public override void NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            // Serialize ConnectionIndexes
            int indexCount = ConnectionIndexes != null ? ConnectionIndexes.Count : 0;
            serializer.SerializeValue(ref indexCount);

            if (serializer.IsReader)
            {
                ConnectionIndexes = new List<int>(indexCount);
                for (int i = 0; i < indexCount; i++)
                {
                    int index = 0;
                    serializer.SerializeValue(ref index);
                    ConnectionIndexes.Add(index);
                }
            }
            else
            {
                foreach (var index in ConnectionIndexes)
                {
                    int tempIndex = index;
                    serializer.SerializeValue(ref tempIndex);
                }
            }

            // Serialize Conditions
            SerializeScriptableObjectList(serializer, ref _conditions);

            // Serialize Event Listeners (using EventListenerSOBase)
            SerializeScriptableObjectList(serializer, ref _eventListeners);

            // Serialize Data
            Data.NetworkSerialize(serializer);
        }

        private void SerializeScriptableObjectList<T>(BufferSerializer<T> serializer, ref List<ScriptableObject> list) where T : IReaderWriter
        {
            int count = list != null ? list.Count : 0;
            serializer.SerializeValue(ref count);

            if (serializer.IsReader)
            {
                list = new List<ScriptableObject>(count);
                for (int i = 0; i < count; i++)
                {
                    string typeId = string.Empty;
                    serializer.SerializeValue(ref typeId);

                    var scriptableObjectType = Type.GetType(typeId);
                    if (scriptableObjectType != null)
                    {
                        var obj = CreateInstance(scriptableObjectType);
                        if (obj is INetworkSerializable serializableObj)
                        {
                            serializableObj.NetworkSerialize(serializer);
                        }
                        list.Add(obj);
                    }
                }
            }
            else
            {
                foreach (var item in list)
                {
                    string typeId = item.GetType().AssemblyQualifiedName;
                    serializer.SerializeValue(ref typeId);

                    if (item is INetworkSerializable serializableObj)
                    {
                        serializableObj.NetworkSerialize(serializer);
                    }
                }
            }
        }
        #endregion
    }
}
