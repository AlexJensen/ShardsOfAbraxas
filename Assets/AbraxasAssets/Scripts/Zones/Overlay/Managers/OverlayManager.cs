using Abraxas.Cards;
using Abraxas.Cards.Controllers;
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
        public ICardController Card { get; set; }
        #endregion

        #region Methods
        public void AddCard(ICardController card)
        {
            card.View.Transform.SetParent(Overlay.transform);
            Card = card;
        }

        public void RemoveCard(ICardController card)
        {
            Card = null;
        }
        #endregion
    }
}