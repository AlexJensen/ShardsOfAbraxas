﻿using Abraxas.Stones.Controllers;
using Abraxas.Stones.Data;
using System;
using UnityEngine;

namespace Abraxas.Stones.Types
{
    [CreateAssetMenu(fileName = "New Trigger StartOfOpponentCombat", menuName = "Abraxas/StoneData/Triggers/Start of Opponent Combat")]
    [Serializable]
    public class TriggerStartOfOpponentCombatDataSO : TriggerStoneDataSO
    {
        [SerializeField]
        BasicStoneData _data = new()
        {
            Info = "At the start of your opponent's combat:"
        };

        public override IStoneData Data { get => _data; set => _data = (BasicStoneData)value; }
        public override Type ControllerType { get; set; } = typeof(Trigger_StartOfOpponentCombat);
    } 
}