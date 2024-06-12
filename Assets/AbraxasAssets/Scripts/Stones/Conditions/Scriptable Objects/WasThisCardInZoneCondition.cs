using Abraxas.Events;
using Abraxas.Zones;
using System;
using UnityEngine;

namespace Abraxas.Stones.Controllers.StoneTypes.Conditions
{
    [CreateAssetMenu(menuName = "Abraxas/Conditions/WasThisCardInZonePreviously")]
    [Serializable]
    public class WasThisCardInZonePreviouslyCondition : Condition<CardChangedZonesEvent>
    {
        #region Dependencies
        public override void Initialize(TriggerStone stone, ICondition condition)
        {
            Zone = ((WasThisCardInZonePreviouslyCondition)condition).Zone;
            base.Initialize(stone, condition);
        }
        #endregion

        #region Fields
        [SerializeField]
        ZoneType _zone;
        #endregion

        #region Properties
        public ZoneType Zone { get => _zone; set => _zone = value; }
        #endregion

        #region Methods
        public override bool IsMet()
        {
            if (Stone.Card.PreviousZone == null) return false;
            return Zone == Stone.Card.PreviousZone.Type;
        }

        public override bool ShouldReceiveEvent(CardChangedZonesEvent eventData)
        {
            if (eventData.Card != Stone.Card) return false;
            return base.ShouldReceiveEvent(eventData);
        }
        #endregion
    }
}