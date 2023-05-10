using Abraxas.Cards.Controllers;
using UnityEngine;

namespace Abraxas.Zones.Overlays
{
    public interface IOverlayManager
    {
        Transform Overlay { get; }
        ICardController Card { get; }

        void AddCard(ICardController card);
        void RemoveCard(ICardController card);
    }
}
