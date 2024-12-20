using UnityEngine;

namespace Abraxas.Stones.Targets
{
    [CreateAssetMenu(fileName = "New Target Integer Value", menuName = "Abraxas/Data/StoneData/Targets/IntegerValue")]
    class Target_IntegerValueSO : TargetSO<int>
    {
        [SerializeField]
        int _target;
        public override int Target
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
