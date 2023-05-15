using Abraxas.Cards.Controllers;
using Abraxas.Cards.Views;
using UnityEngine;

namespace Abraxas.Zones.Overlays
{
    public interface IOverlayManager
    {
        Transform Overlay { get; }
        ICardView Card { get; }

        void SetCard(ICardView card);
        void ClearCard(ICardView card);
    }
}
