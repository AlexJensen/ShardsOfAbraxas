using Abraxas.Behaviours.Cards;
using Abraxas.Core;
using UnityEngine;

namespace Abraxas.Behaviours.Zones.Drags
{ 
    public class DragManager : Singleton<DragManager>
    {
        public RectTransform templateCardRect;
        public Card card;
    }
}