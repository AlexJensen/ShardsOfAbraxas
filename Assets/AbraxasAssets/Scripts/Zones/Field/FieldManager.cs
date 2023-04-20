using System.Collections;
using UnityEngine;
using Abraxas.Cards;

namespace Abraxas.Zones.Fields
{
    public class FieldManager : MonoBehaviour, IFieldManager
    {
        #region Fields
        [SerializeField]
        Field _field;
        #endregion

        #region Methods
        public void AddCard(Card card, Cell cell)
        {
            _field.AddToField(card, cell);
        }

        public void AddCard(Card card, Vector2Int fieldPos)
        {
            _field.AddToField(card, fieldPos);
        }

        public void RemoveCard(Card card)
        {
            _field.RemoveCard(card);
        }

        public IEnumerator StartCombat()
        {
            yield return _field.StartCombat();
        }

        public IEnumerator MoveCardAndFight(Card card, Vector2Int movement)
        {
            yield return _field.MoveCardAndFight(card, movement);
        }

        public IEnumerator MoveCardToCell(Card card, Cell cell)
        {
            yield return _field.MoveCardToCell(card, cell);
        }

        public IEnumerator MoveCardToCell(Card card, Vector2Int fieldPos)
        {
            yield return _field.MoveCardToCell(card, fieldPos);
        }

        public Vector2 GetCellDimensions()
        {
            return _field.GetCellDimensions();
        }
        #endregion
    }
}