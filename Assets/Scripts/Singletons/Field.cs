using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Row
{
    public List<Cell> cells;

    public Row()
    {
        cells = new List<Cell>();
    }
}

public class Field : Singleton<Field>
{
    List<Card> cards;

    List<Row> field;

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

    public void MoveToCombat()
    {
        StartCoroutine(Combat());
    }

    IEnumerator Combat()
    {
        foreach (Card card in cards)
        {
            yield return StartCoroutine(card.OnCombat());
        }
    }

    public void AddToField(Card card, Cell cell)
    {
        cards.Add(card);
        card.zone = Zone.PLAY;
        cell.AddCard(card);
    }

    public IEnumerator MoveCard(Card card, Vector2Int movement)
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
            yield return StartCoroutine(card.MoveTo(field[destination.y].cells[destination.x].RectTransform.position, Card.MOVE_TIME));
            field[destination.y].cells[destination.x].AddCard(card);
        }
    }
}
