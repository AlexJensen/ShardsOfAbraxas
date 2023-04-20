using Abraxas.Cards;
using UnityEngine;

namespace Abraxas.Zones.Overlays
{ 
    public class OverlayManager : MonoBehaviour, IOverlayManager
    {
        #region Fields
        [SerializeField]
        Transform _overlay;
        #endregion

        #region Properties
        public Transform Overlay { get => _overlay; }
        public Card Card { get; set; }
        #endregion

        #region Methods
        public void AddCard(Card card)
        {
            card.transform.SetParent(Overlay.transform);
            Card = card;
        }

        public void RemoveCard(Card card)
        {
            Card = null;
        }
        #endregion
    }
}