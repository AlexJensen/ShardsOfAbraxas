using Abraxas.Stones.Controllers;
using Abraxas.Stones.Data;
using System;
using UnityEngine;

namespace Abraxas.Stones.Types
{
    [CreateAssetMenu(fileName = "New Trigger StartOfOpponentEndTurn", menuName = "Abraxas/StoneData/Triggers/Start of Opponent End Turn")]
    [Serializable]
    public class TriggerStartOfOpponentEndTurnDataSO : TriggerStoneDataSO
    {
        [SerializeField]
        BasicStoneData _data = new()
        {
            Info = "At the end of your opponent's turn:"
        };

        public override IStoneData Data { get => _data; set => _data = (BasicStoneData)value; }
        public override Type ControllerType { get; set; } = typeof(Trigger_StartOfOpponentEndTurn);
    } 
}
