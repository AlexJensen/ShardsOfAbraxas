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

        #endregion

        #region Unity Methods
        protected override void Awake()
        {
            base.Awake();
            cards = new List<Card>();
            field = new List<Row>();

            foreach (Transform rowT in transform)
            {
                Row row = new();
                field.Add(row);
                foreach (Transform cellT in rowT.transform)
                {
                    Cell cell = cellT.GetComponent<Cell>();
                    row.cells.Add(cell);
                    cell.fieldPos = new Vector2Int(row.cells.IndexOf(cell), field.IndexOf(row));
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
            cards.Add(card);
            card.Zone = Card.Zones.PLAY;
            cell.AddCard(card);
            EventManager.Instance.CardEnteredField(card);
        }

        public void AddToField(Card card, Vector2Int fieldPos)
        {
            AddToField(card, field[fieldPos.y].cells[fieldPos.x]);
        }

        public void RemoveFromField(Card card)
        {
            cards.Remove(card);
            card.Cell.RemoveCard(card);
        }

        /// <summary>
        /// Initiate combat for all cards on the field.
        /// </summary>
        /// <returns>Execution will continue after combat is completed.</returns>
        public IEnumerator StartCombat()
        {
            Card[] activeCards = cards.Where(s => s.Controller == GameManager.Instance.ActivePlayer).Reverse().ToArray();
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
            Vector2Int destination = new(Math.Max(0, Math.Min(field[0].cells.Count - 1, card.FieldPosition.x + movement.x)), Math.Max(0, Math.Min(field[0].cells.Count - 1, card.FieldPosition.y + movement.y)));
            Card collided = null;
            if (movement.x > 0)
            {
                for (int i = card.FieldPosition.x + 1; i <= destination.x; i++)
                {
                    if (field[card.FieldPosition.y].cells[i].Cards.Count > 0)
                    {
                        destination.x = i - 1;
                        collided = field[card.FieldPosition.y].cells[i].Cards[0];
                        break;
                    }
                }
            }
            else
            {
                for (int i = card.FieldPosition.x - 1; i >= destination.x; i--)
                {
                    IEnumerable<Card> activeCards = field[card.FieldPosition.y].cells[i].Cards.Where(x => x.isActiveAndEnabled);
                    if (activeCards.Count() > 0)
                    {
                        destination.x = i + 1;
                        collided = field[card.FieldPosition.y].cells[i].Cards[0];
                        break;
                    }
                }
            }

            if (destination != card.FieldPosition)
            {
                field[card.FieldPosition.y].cells[card.FieldPosition.x].Cards.Remove(card);
                card.transform.SetParent(DragManager.Instance.transform);
                yield return StartCoroutine(card.MoveTo(field[destination.y].cells[destination.x].RectTransform.position, Card.MOVEMENT_ON_FIELD_TIME));
                field[destination.y].cells[destination.x].AddCard(card);
                EventManager.Instance.CardMove(card);
            }
            if (collided != null)
            {
                yield return StartCoroutine(card.Fight(collided));
            }
        }

        public IEnumerator MoveToFieldPosition(Card card, Vector2Int fieldPos)
        {
            yield return card.MoveToCell(field[fieldPos.y].cells[fieldPos.x]);
        }
        #endregion
    }
}