using Abraxas.Behaviours.Cards;
using Abraxas.Behaviours.Data;
using UnityEngine;

namespace Abraxas.Behaviours.Stones
{
    public abstract class Stone : MonoBehaviour
    {
        private int cost;
        private string info;
        private StoneData.StoneType stoneType;

        [HideInInspector]
        public Card card;

        public virtual int Cost { get => cost; set => cost = value; }
        public virtual string Info { get => info; set => info = value; }
        public virtual StoneData.StoneType StoneType { get => stoneType; set => stoneType = value; }
    }
}