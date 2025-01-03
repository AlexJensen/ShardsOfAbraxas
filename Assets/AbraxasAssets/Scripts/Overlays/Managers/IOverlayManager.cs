using Abraxas.Cards.Controllers;
using Abraxas.Core;
using UnityEngine;

namespace Abraxas.Zones.Overlays.Managers
{
    /// <summary>
    /// IOverlayManager is an interface for managing overlays.
    /// </summary>
    public interface IOverlayManager : IManager
    {
        Transform Overlay { get; }

        void SetCard(ICardController card);
        void ClearCard(ICardController card);
        bool HasCard(ICardController card);
    }
}
