using Abraxas.Stones.Controllers;
using Abraxas.Stones.Data;
using System;
using UnityEngine;

namespace Abraxas.Stones.Types
{
    [CreateAssetMenu(fileName = "New Trigger StartOfTurn", menuName = "Abraxas/StoneData/Triggers/Start of Turn")]
    [Serializable]
    public class TriggerStartOfTurnDataSO : TriggerStoneDataSO
    {
        [SerializeField]
        BasicStoneData _data = new()
        {
            Info = "At the start of the turn:"
        };

        public override IStoneData Data { get => _data; set => _data = (BasicStoneData)value; }
        public override Type ControllerType { get; set; } = typeof(Trigger_StartOfTurn);
    }
}
