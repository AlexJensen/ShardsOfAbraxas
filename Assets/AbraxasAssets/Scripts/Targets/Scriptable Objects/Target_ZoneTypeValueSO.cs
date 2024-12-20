using Abraxas.Zones;
using UnityEngine;

namespace Abraxas.Stones.Targets
{
    [CreateAssetMenu(fileName = "New Target Zone Value", menuName = "Abraxas/Data/StoneData/Targets/ZoneValue")]
    class Target_ZoneTypeValueSO : TargetSO<ZoneType>
    {
        [SerializeField]
        ZoneType _target;
        public override ZoneType Target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;
            }
        }
    }
}
