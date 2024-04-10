using Abraxas.Cards.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Abraxas.Decks.Scriptable_Objects
{
    [CreateAssetMenu(fileName = "NewDeck", menuName = "Abraxas/Deck")]
    public class DeckDataSO : ScriptableObject
    {
        public List<CardData> cards;
    }
}
