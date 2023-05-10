using Abraxas.Cards;
using Abraxas.Cards.Controllers;
using Abraxas.Zones.Overlays;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Abraxas.Zones
{
    public enum Zones
    {
        DECK, DRAG, HAND, PLAY, GRAVEYARD, BANISHED
    }
    public abstract class ZoneView : MonoBehaviour
    {
        #region Dependencies
        IOverlayManager _overlayManager;

        [Inject]
        public void Construct(IOverlayManager overlayManager)
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
        public virtual IEnumerator MoveCardToZone(ICardController card, int index = 0)
        {
            _overlayManager.AddCard(card);
            yield return card.View.RectTransformMover.MoveToFitRectangle(_cards, MoveCardTime);
            AddCard(card, index);
        }

        public void AddCard(ICardController card, int index = 0)
        {
            card.View.Transform.localScale = Vector3.zero;
            card.View.Transform.position = transform.position;
            card.Zone = ZoneType;
            card.View.Transform.SetParent(Cards.transform);
            card.View.Transform.SetSiblingIndex(index);
        }

        public ICardController RemoveCard(int index = 0)
        {
            ICardController card = Cards.GetChild(index).GetComponent<ICardController>();
            card.View.Transform.localScale = Vector3.one;
            card.View.Transform.position = transform.position;
            return card;
        }
        #endregion
    }
}
