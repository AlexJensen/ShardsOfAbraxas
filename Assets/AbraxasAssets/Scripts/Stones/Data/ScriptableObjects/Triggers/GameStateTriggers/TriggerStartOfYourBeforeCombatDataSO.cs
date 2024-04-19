using Abraxas.Stones.Controllers;
using Abraxas.Stones.Data;
using System;
using UnityEngine;

namespace Abraxas.Stones.Types
{
    [CreateAssetMenu(fileName = "New Trigger StartOfYourBeforeCombat", menuName = "Abraxas/StoneData/Triggers/Start of Your Before Combat")]
    [Serializable]
    public class TriggerStartOfYourBeforeCombatDataSO : TriggerStoneDataSO
    {
        [SerializeField]
        BasicStoneData _data = new()
        {
            Info = "At the start of your first main phase:"
        };

        public override IStoneData Data { get => _data; set => _data = (BasicStoneData)value; }
        public override Type ControllerType { get; set; } = typeof(Trigger_StartOfYourBeforeCombat);
    } 
}
