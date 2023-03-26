using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        card.zone = Card.Zone.DEAD;
    }
}
