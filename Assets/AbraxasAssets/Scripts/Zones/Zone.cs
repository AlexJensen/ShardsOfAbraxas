using Abraxas.Behaviours.Cards;
using Abraxas.Behaviours.Zones.Fields;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Abraxas.Behaviours.Zones
{
    public abstract class Zone : NetworkBehaviour
    {
        #region Dependency Injections
        [Inject] readonly FieldManager _fieldManager;
        #endregion

        #region Fields
        [SerializeField]
        Transform _cards;
        #endregion

        #region Properties
        RectTransform RectTransform => (RectTransform)transform;
        #endregion

        public abstract ZoneManager.Zones ZoneType { get; }
        public Transform Cards { get => _cards; set => _cards = value; }

        private void Update()
        {
            foreach (Transform card in Cards)
            {
                card.localScale = Vector3.zero;
            }
        }

        internal virtual void AddCard(Card card, int index = 0)
        {
            card.transform.localScale = Vector3.zero;
            card.transform.position = transform.position;
            card.transform.SetParent(Cards);
            card.Zone = ZoneType;
        }

        public virtual IEnumerator MoveCardToZone(Card card)
        {
            yield return card.MoveToFitRectangle(RectTransform);
            _fieldManager.RemoveFromField(card);
            AddCard(card);
        }
    }
}
