using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Field : Singleton<Field>
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
        card.zone = Card.Zone.PLAY;
        cell.AddCard(card);
        Events.Instance.CardEnteredField(card);
    }

    public void AddToField(Card card, Vector2Int fieldPos)
    {
        AddToField(card, field[fieldPos.y].cells[fieldPos.x]);
    }

    /// <summary>
    /// Initiate combat for all cards on the field.
    /// </summary>
    /// <returns>Execution will continue after combat is completed.</returns>
    public IEnumerator StartCombat()
    {
        foreach (Card card in cards.Where(s => s.Controller == Game.Instance.ActivePlayer))
        {
            yield return StartCoroutine(card.OnCombat());
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
        Vector2Int destination = new(Math.Max(0, Math.Min(field[0].cells.Count - 1, card.fieldPos.x + movement.x)), Math.Max(0, Math.Min(field[0].cells.Count - 1, card.fieldPos.y + movement.y)));
        if (movement.x > 0)
        {
            for (int i = card.fieldPos.x + 1; i <= destination.x; i++)
            {
                if (field[card.fieldPos.y].cells[i].Cards.Count > 0)
                {
                    destination.x = i - 1;
                    break;
                }
            }
        }
        else
        {
            for (int i = card.fieldPos.x - 1; i >= destination.x; i--)
            {
                if (field[card.fieldPos.y].cells[i].Cards.Count > 0)
                {
                    destination.x = i + 1;
                    break;
                }
            }
        }

        if (destination != card.fieldPos)
        {
            field[card.fieldPos.y].cells[card.fieldPos.x].Cards.Remove(card);
            card.transform.SetParent(Drag.Instance.transform);
            yield return StartCoroutine(card.MoveTo(field[destination.y].cells[destination.x].RectTransform.position, Card.MOVEMENT_ON_FIELD_TIME));
            field[destination.y].cells[destination.x].AddCard(card);
            Events.Instance.CardMove(card);
        }
    }

    public IEnumerator MoveToFieldPosition(Card card, Vector2Int fieldPos)
    {
        yield return card.MoveToCell(field[fieldPos.y].cells[fieldPos.x]);
    }
    #endregion
}
