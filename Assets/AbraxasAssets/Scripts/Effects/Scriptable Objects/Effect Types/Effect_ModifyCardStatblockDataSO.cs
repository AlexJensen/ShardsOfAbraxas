using Abraxas.Cards.Controllers;
using Abraxas.Stones.Data;
using Abraxas.Stones.Targets;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Abraxas.Stones.Types
{
    [CreateAssetMenu(fileName = "New Effect ModifyTargetCardGroupStatBlock", menuName = "Abraxas/StoneData/Effects/Modify Target Card Group Stat Block")]
    [Serializable]
    public class Effect_ModifyCardGroupStatblockDataSO : EffectStoneSO
    {
        #region Fields
        [SerializeField]
        ModifyCardStatBlockData _data = new();
        [SerializeField]
        TargetSO<List<ICardController>> _targets;
        #endregion

        #region Properties
        public override IStoneData Data { get => _data; set => _data = (ModifyCardStatBlockData)value; }
        public override Type ControllerType { get; set; } = typeof(Effect_ModifyCardGroupStatblockDataSO);
        #endregion
    }
}