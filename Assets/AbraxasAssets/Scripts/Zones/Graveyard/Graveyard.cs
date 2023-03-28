using Abraxas.Behaviours.Cards;
using UnityEngine;

namespace Abraxas.Behaviours.Zones.Graveyards
{
    public class Graveyard : MonoBehaviour
    {
        [SerializeField]
        Transform cards;

        private void Update()
        {
            foreach (Transform card in cards)
            {
                card.localScale = Vector3.zero;
            }
        }

        internal void AddCard(Card card, int index = 0)
        {
            card.transform.localScale = Vector3.zero;
            card.transform.position = transform.position;
            card.transform.SetParent(cards);
            card.Zone = Card.Zones.DEAD;
        }
    }
}