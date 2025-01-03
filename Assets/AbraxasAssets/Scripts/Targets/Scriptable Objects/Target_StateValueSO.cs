using Abraxas.GameStates;
using UnityEngine;

namespace Abraxas.Stones.Targets
{
    [CreateAssetMenu(fileName = "New Target Game State Value", menuName = "Abraxas/Data/StoneData/Targets/GameStateValue")]
    class Target_StateValueSO : TargetSO<GameStates.GameStates>
    {
        [SerializeField]
        GameStates.GameStates _target;
        public override GameStates.GameStates Target
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
