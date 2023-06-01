using Abraxas.Cards.Controllers;
using Abraxas.Cards.Views;
using UnityEngine;

namespace Abraxas.Zones.Overlays.Managers
{
    public interface IOverlayManager
    {
        Transform Overlay { get; }

        void SetCard(ICardView card);
        void ClearCard(ICardView card);
        bool HasCard(ICardView card);
    }
}
