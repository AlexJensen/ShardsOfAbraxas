using Abraxas.Behaviours.Cards;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Abraxas.Behaviours.Zones
{
    public abstract class Zone : NetworkBehaviour
    {
        [SerializeField]
        Transform cards;

        public abstract ZoneManager.Zones ZoneType { get; }

        private void Update()
        {
            foreach (Transform card in cards)
            {
                card.localScale = Vector3.zero;
            }
        }

        internal virtual void AddCard(Card card, int index = 0)
        {
            card.transform.localScale = Vector3.zero;
            card.transform.position = transform.position;
            card.transform.SetParent(cards);
            card.Zone = ZoneType;
        }
    }
}
