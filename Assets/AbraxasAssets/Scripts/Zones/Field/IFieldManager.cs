using Abraxas.Cards;
using System.Collections;
using UnityEngine;

namespace Abraxas.Zones.Fields
{
    public interface IFieldManager
    {
        void RemoveCard(Card card);
        IEnumerator MoveCardToCell(Card card, Vector2Int fieldPosition);
        void AddCard(Card card, Vector2Int fieldPosition);
    }
}
