using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class Cell : MonoBehaviour
{
    RectTransform rectTransform;
    List<Card> cards;

    public GameData.Players player;
    public Vector2Int fieldPos;

    public List<Card> Cards { get => cards; }
    public RectTransform RectTransform { get => rectTransform; }

    #region Unity Methods
    private void Awake()
    {
        cards = new List<Card>();
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Card card in GetComponentsInChildren<Card>())
        {
            FitCardInCell(card);
        }
    }
    #endregion

    #region Card Methods
    internal void FitCardInCell(Card card)
    {
        card.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, RectTransform.rect.width);
        card.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, RectTransform.rect.height);
    }

    internal void AddCard(Card card)
    {
        Cards.Add(card);
        card.transform.SetParent(transform);
        card.fieldPos = fieldPos;
        FitCardInCell(card);
    }
    #endregion
}
