using Abraxas.Behaviours.Cards;
using Abraxas.Behaviours.Game;
using System.Collections.Generic;
using UnityEngine;

namespace Abraxas.Behaviours.Zones.Fields
{
    /// <summary>
    /// Controller for a Cell square on the field and contains methods to store card positions and fit card frames into the cell.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class Cell : MonoBehaviour
    {
        #region Fields
        RectTransform rectTransform;
        List<Card> cards;
        public GameManager.Player player;
        public Vector2Int fieldPos;
        #endregion

        #region Properties
        public List<Card> Cards { get => cards; }
        public RectTransform RectTransform { get => rectTransform; }
        #endregion

        #region Unity Methods
        private void Awake()
        {
            cards = new List<Card>();
            rectTransform = (RectTransform)transform;
        }

        void Update()
        {
            foreach (Card card in GetComponentsInChildren<Card>())
            {
                FitCardInCell(card);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Rescales and repositions a card to fit within this cell.
        /// </summary>
        /// <param name="card">Card to fit.</param>
        void FitCardInCell(Card card)
        {
            ((RectTransform)card.transform).SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, RectTransform.rect.width);
            ((RectTransform)card.transform).SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, RectTransform.rect.height);
        }

        /// <summary>
        /// Accessor to add a card to this cell.
        /// </summary>
        /// <param name="card">Card to add.</param>
        public void AddCard(Card card)
        {
            Cards.Add(card);
            card.Cell = this;
            card.transform.SetParent(transform);
            card.FieldPosition = fieldPos;
            FitCardInCell(card);
        }

        public void RemoveCard(Card card)
        {
            Cards.Remove(card);
            card.Cell = null;
        }
        #endregion
    }
}