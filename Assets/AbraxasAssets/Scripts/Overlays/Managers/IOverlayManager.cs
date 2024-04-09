using Abraxas.Cards.Controllers;
using Abraxas.Cards.Views;
using UnityEngine;

namespace Abraxas.Zones.Overlays.Managers
{
    public interface IOverlayManager
    {
        Transform Overlay { get; }

        void SetCard(ICardController card);
        void ClearCard(ICardController card);
        bool HasCard(ICardController card);
    }
}
