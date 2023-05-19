using Abraxas.Cards;
using Abraxas.Cards.Controllers;
using Abraxas.Cards.Views;
using System.Collections.Generic;
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
        List<ICardView> Cards { get;} = new();
        #endregion

        #region Methods
        public void SetCard(ICardView card)
        {
            card.Transform.SetParent(Overlay.transform);
            Cards.Add(card);
        }

        public void ClearCard(ICardView card)
        {
            Cards.Remove(card);
        }

        public bool HasCard(ICardView card)
        {
            return Cards.Contains(card);
        }
        #endregion
    }
}