using Abraxas.Behaviours.Cards;
using Abraxas.Behaviours.Events;
using Abraxas.Behaviours.Game;
using Abraxas.Behaviours.Zones.Drags;
using Abraxas.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Abraxas.Behaviours.Zones.Fields
{
    public class FieldManager : Singleton<FieldManager>
    {
        #region Constants
        public class Row
        {
            public List<Cell> cells;
            public Row()
            {
                cells = new List<Cell>();
            }
        }
        #endregion

        #region Fields
        List<Card> cards;
        List<Row> field;

        public List<Card> Cards { get => cards; set => cards = value; }
        public List<Row> Field { get => field; set => field = value; }

        #endregion

        #region Unity Methods
        protected void Awake()
        {
            Cards = new List<Card>();
            Field = new List<Row>();

            foreach (Transform rowT in transform)
            {
                Row row = new();
                Field.Add(row);
                foreach (Transform cellT in rowT.transform)
                {
                    Cell cell = cellT.GetComponent<Cell>();
                    row.cells.Add(cell);
                    cell.fieldPos = new Vector2Int(row.cells.IndexOf(cell), Field.IndexOf(row));
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds a card to a cell on the field.
        /// </summary>
        /// <param name="card">Card to add.</param>
        /// <param name="cell">Cell to insert card.</param>
        public void AddToField(Card card, Cell cell)
        {
            Cards.Add(card);
            card.Zone = ZoneManager.Zones.PLAY;
            cell.AddCard(card);
            EventManager.Instance.CardEnteredField(card);
        }

        public void AddToField(Card card, Vector2Int fieldPos)
        {
            AddToField(card, Field[fieldPos.y].cells[fieldPos.x]);
        }

        public void RemoveFromField(Card card)
        {
            Cards.Remove(card);
            card.Cell.RemoveCard(card);
        }

        /// <summary>
        /// Initiate combat for all cards on the field.
        /// </summary>
        /// <returns>Execution will continue after combat is completed.</returns>
        public IEnumerator StartCombat()
        {
            Card[] activeCards = Cards.Where(s => s.Controller == GameManager.Instance.ActivePlayer).Reverse().ToArray();
            for (int i = activeCards.Length - 1; i >= 0; i--)
            {

                yield return StartCoroutine(activeCards[i].OnCombat());
            }
        }

        /// <summary>
        /// Attempts to move a card a specified number of cells on the field, stopping short if the card collides with another card or would move outside the board, and initiating combat with the colliding card if it 
        /// is an enemy.
        /// </summary>
        /// <param name="card">Card to move.</param>
        /// <param name="movement">Movement vector to travel.</param>
        /// <returns>Execution will continue after movement and combat is complete.</returns>
        public IEnumerator MoveCardAndFight(Card card, Vector2Int movement)
        {
            // first clamp the destination to inside the board.
            Vector2Int destination = new Vector2Int(
                Mathf.Clamp(card.FieldPosition.x + movement.x, 0, Field[0].cells.Count - 1),
                Mathf.Clamp(card.FieldPosition.y + movement.y, 0, Field.Count - 1));

            Card collided = null;
            IEnumerable<Card> activeCards = Enumerable.Empty<Card>();
            for (int i = card.FieldPosition.x + Math.Sign(movement.x); i != destination.x + Math.Sign(movement.x); i += Math.Sign(movement.x))
            {
                activeCards = Field[card.FieldPosition.y].cells[i].Cards.Where(x => x.isActiveAndEnabled);
                if (activeCards.Count() > 0)
                {
                    destination.x = i - Math.Sign(movement.x);
                    collided = Field[card.FieldPosition.y].cells[i].Cards[0];
                    break;
                }
            }

            if (destination != card.FieldPosition)
            {
                Field[card.FieldPosition.y].cells[card.FieldPosition.x].Cards.Remove(card);
                card.transform.SetParent(DragManager.Instance.transform);
                yield return StartCoroutine(card.MoveTo(Field[destination.y].cells[destination.x].RectTransform.position, Card.MOVEMENT_ON_FIELD_TIME));
                Field[destination.y].cells[destination.x].AddCard(card);
                EventManager.Instance.CardMove(card);
                if (Field[destination.y].cells[destination.x].player != card.Controller && Field[destination.y].cells[destination.x].player != GameManager.Player.Neutral)
                {
                    yield return StartCoroutine(card.PassHomeRow());
                }
            }
            if (collided != null)
            {
                yield return StartCoroutine(card.Fight(collided));
            }
        }

        public IEnumerator MoveToFieldPosition(Card card, Vector2Int fieldPos)
        {
            yield return card.MoveToCell(Field[fieldPos.y].cells[fieldPos.x]);
        }
        #endregion
    }
}