using Abraxas.Cards;
using Abraxas.Zones.Overlays;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Abraxas.Zones
{
    public enum Zones
    {
        DECK, DRAG, HAND, PLAY, GRAVEYARD, BANISHED
    }
    public abstract class Zone : NetworkBehaviour
    {
        #region Dependencies
        IOverlayManager _overlayManager;

        [Inject]
        void Construct(IOverlayManager overlayManager)
        {
            _overlayManager = overlayManager;
        }
        #endregion

        #region Fields
        [SerializeField]
        RectTransform _cards;
        #endregion

        #region Properties
        public abstract Zones ZoneType { get; }
        public Transform Cards { get => _cards; }
        public abstract float MoveCardTime { get; }
        #endregion

        #region Methods
        public virtual IEnumerator MoveCardToZone(Card card, int index = 0)
        {
            _overlayManager.AddCard(card);
            yield return card.RectTransformMover.MoveToFitRectangle(_cards, MoveCardTime);
        }
        #endregion
    }
}
