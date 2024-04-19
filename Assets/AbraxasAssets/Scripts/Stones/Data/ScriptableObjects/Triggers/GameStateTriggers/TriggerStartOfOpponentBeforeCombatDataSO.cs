using Abraxas.Stones.Controllers;
using Abraxas.Stones.Data;
using System;
using UnityEngine;

namespace Abraxas.Stones.Types
{
    [CreateAssetMenu(fileName = "New Trigger StartOfOpponentBeforeCombat", menuName = "Abraxas/StoneData/Triggers/Start of Opponent Before Combat")]
    [Serializable]
    public class TriggerStartOfOpponentBeforeCombatDataSO : TriggerStoneDataSO
    {
        [SerializeField]
        BasicStoneData _data = new()
        {
            Info = "At the start of your opponent's first main phase:"
        };

        public override IStoneData Data { get => _data; set => _data = (BasicStoneData)value; }
        public override Type ControllerType { get; set; } = typeof(Trigger_StartOfOpponentBeforeCombat);
    } 
}
