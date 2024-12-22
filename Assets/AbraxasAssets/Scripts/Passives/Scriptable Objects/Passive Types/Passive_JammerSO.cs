using Abraxas.Stones.Data;
using System;
using UnityEngine;

namespace Abraxas.Passives
{
    [CreateAssetMenu(fileName = "New Passive Jammer", menuName = "Abraxas/Data/StoneData/Passives/Jammer")]
    [Serializable]
    internal class Passive_JammerSO : PassiveStoneSO
    {
        #region Fields
        [SerializeField]
        BasicStoneData _data = new();
        #endregion

        #region Properties

        #endregion
        public override IStoneData Data { get => _data; set => _data = (BasicStoneData)value; }
        public override Type ControllerType { get; set; } = typeof(Passive_Jammer);
    }
}
