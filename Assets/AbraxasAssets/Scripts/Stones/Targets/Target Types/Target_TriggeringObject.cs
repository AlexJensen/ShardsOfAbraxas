using Abraxas.Events;
using System.Collections;
using UnityEngine;

namespace Abraxas.Stones.Targets
{

    [CreateAssetMenu(fileName = "New Target TriggeringObject", menuName = "Abraxas/StoneData/Targets/TriggeringObject")]
    class Target_TriggeringObject<T> : TargetSO<T>, IGameEventListener<T>
    {
        private T _triggeringObject;

        public override T Target
        {
            get => _triggeringObject;
            set => _triggeringObject = value;
        }

        public IEnumerator OnEventRaised(T eventData)
        {
            _triggeringObject = eventData;
            yield return null;
        }

        public bool ShouldReceiveEvent(T eventData)
        {
            return true;
        }
    }
}
