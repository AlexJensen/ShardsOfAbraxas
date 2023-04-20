using Abraxas.Cards;
using System.Collections.Generic;
using UnityEngine;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Fields
{
    /// <summary>
    /// Controller for a Cell square on the field and contains methods to store card positions and fit card frames into the cell.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class Cell : MonoBehaviour
    {
        #region Fields
        RectTransform _rectTransform;
        List<Card> _cards;
        [SerializeField]
        Player _player;
        Vector2Int _fieldPosition;
        #endregion

        #region Properties
        public List<Card> Cards { get => _cards; }
        public RectTransform RectTransform { get => _rectTransform; }
        public Player Player { get => _player; }
        public Vector2Int FieldPosition { get => _fieldPosition; set => _fieldPosition = value; }
        #endregion

        #region Unity Methods
        private void Awake()
        {
            _cards = new List<Card>();
            _rectTransform = (RectTransform)transform;
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
            card.FieldPosition = FieldPosition;
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