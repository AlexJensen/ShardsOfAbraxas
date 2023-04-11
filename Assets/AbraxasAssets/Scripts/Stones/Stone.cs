using Abraxas.Behaviours.Cards;
using Abraxas.Behaviours.Data;
using UnityEngine;

namespace Abraxas.Behaviours.Stones
{
    public abstract class Stone : MonoBehaviour
    {
        [HideInInspector]
        public Card card;

        public abstract int Cost { get;  set; }
        public abstract string Info { get; set; }
        public abstract StoneData.StoneType StoneType { get; set; }
    }
}