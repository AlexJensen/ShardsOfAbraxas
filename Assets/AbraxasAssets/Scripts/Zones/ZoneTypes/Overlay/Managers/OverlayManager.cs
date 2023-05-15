using Abraxas.Cards;
using Abraxas.Cards.Controllers;
using Abraxas.Cards.Views;
using UnityEngine;

namespace Abraxas.Zones.Overlays.Managers
{ 
    public class OverlayManager : MonoBehaviour, IOverlayManager
    {
        #region Fields
        [SerializeField]
        Transform _overlay;
        #endregion

        #region Properties
        public Transform Overlay { get => _overlay; }
        public ICardView Card { get; private set; }
        #endregion

        #region Methods
        public void SetCard(ICardView card)
        {
            card.Transform.SetParent(Overlay.transform);
            Card = card;
        }

        public void ClearCard(ICardView card)
        {
            Card = null;
        }
        #endregion
    }
}