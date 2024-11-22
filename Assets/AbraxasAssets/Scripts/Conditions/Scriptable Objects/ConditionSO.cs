using Abraxas.Stones.Controllers;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Abraxas.Stones.Conditions
{
    public abstract class ConditionSO : ScriptableObject, ICondition 
    {
        protected IStoneController Stone;
        public abstract void Initialize(IStoneController stoneController, DiContainer container);
        public abstract bool IsMet();
        internal abstract void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter;

        public class Factory : PlaceholderFactory<ConditionSO, IStoneController, ICondition> { }
    }
}
