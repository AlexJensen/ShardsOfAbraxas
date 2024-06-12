using Abraxas.Events;
using Abraxas.Zones;
using System;
using UnityEngine;

namespace Abraxas.Stones.Controllers.StoneTypes.Conditions
{
    [CreateAssetMenu(menuName = "Abraxas/Conditions/IsThisCardInZone")]
    [Serializable]
    public class IsThisCardInZoneCondition : Condition<CardChangedZonesEvent>
    {
        #region Dependencies
        override public void Initialize(TriggerStone stone, ICondition condition)
        {
            Zone = ((IsThisCardInZoneCondition)condition).Zone;
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
            return Zone == Stone.Card.Zone.Type;
        }

        public override bool ShouldReceiveEvent(CardChangedZonesEvent eventData)
        {
            if (eventData.Card != Stone.Card) return false;
            return base.ShouldReceiveEvent(eventData);
        }
        #endregion
    }
}