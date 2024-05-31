using Abraxas.Stones.Controllers;
using Abraxas.Stones.Data;
using System;
using UnityEngine;

namespace Abraxas.Stones.Types
{
    [CreateAssetMenu(fileName = "New Effect ModifyCardStatBlock", menuName = "Abraxas/StoneData/Effects/Modify Card Stat Block")]
    [Serializable]
    public class EffectModifyCardStatblockDataSO : EffectStoneDataSO
    {
        [SerializeField]
        ModifyCardStatBlockData _data = new()
        {
            Info = "Modify a card's stat block."
        };
        public override IStoneData Data { get => _data; set => _data = (ModifyCardStatBlockData)value; }
        public override Type ControllerType { get; set; } = typeof(Effect_ModifyCardStatBlock);
    }
}