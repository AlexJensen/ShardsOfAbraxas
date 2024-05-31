using Abraxas.Stones.Controllers;
using Abraxas.Stones.Data;
using System;
using UnityEngine;

namespace Abraxas.Stones.Types
{
    [CreateAssetMenu(fileName = "New Effect DrawCardFromLibrary", menuName = "Abraxas/StoneData/Effects/Draw Card From Library")]
    [Serializable]
    public class EffectDrawCardFromDeckDataSO : EffectStoneDataSO
    {
        [SerializeField]
        BasicStoneData _data = new()
        {
            Info = "Draw a card."
        };
        public override IStoneData Data { get => _data; set => _data = (BasicStoneData)value; }
        public override Type ControllerType { get; set; } = typeof(Effect_DrawCardFromDeck);
    }
}
