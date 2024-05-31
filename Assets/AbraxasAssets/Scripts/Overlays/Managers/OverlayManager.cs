using Abraxas.Cards.Controllers;
using System.Collections.Generic;
using UnityEngine;

namespace Abraxas.Zones.Overlays.Managers
{

    /// <summary>
    /// OverlayManager is a class that manages the overlay zone.
    /// </summary>
	public class OverlayManager : MonoBehaviour, IOverlayManager
    {
        #region Fields
        [SerializeField]
        Transform _overlay;
        #endregion

        #region Properties
        public Transform Overlay { get => _overlay; }

        List<ICardController> Cards { get; } = new();

        #endregion

        #region Methods
        public void SetCard(ICardController card)
        {
            card.TransformManipulator.Transform.SetParent(Overlay.transform);
            Cards.Add(card);
        }

        public void ClearCard(ICardController card)
        {
            Cards.Remove(card);
        }

        public bool HasCard(ICardController card)
        {
            return Cards.Contains(card);
        }
        #endregion
    }
}