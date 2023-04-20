using Abraxas.Cards;
using UnityEngine;

namespace Abraxas.Zones.Overlays
{
    public interface IOverlayManager
    {
        Transform Overlay { get; }
        Card Card { get; }

        void AddCard(Card card);
        void RemoveCard(Card card);
    }
}
