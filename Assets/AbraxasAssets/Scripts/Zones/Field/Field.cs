using Abraxas.Cards;
using Abraxas.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Fields
{
    public class Field : Zone
    {
        #region Settings
        Settings _settings;
        public class Settings
        {
            public float MoveCardToFieldTime;
            public float MoveCardOnFieldTime;
        }
        #endregion

        #region Dependencies
        IGameManager _gameManager;
        [Inject]
        public void Construct(Settings settings, IGameManager gameManager)
        {
            _settings = settings;
            _gameManager = gameManager;
        }
        #endregion

        #region Fields
        List<Card> _cards;
        List<Row> _fieldGrid;
        #endregion

        #region Properties
        public List<Card> CardList { get => _cards; set => _cards = value; }
        public List<Row> FieldGrid { get => _fieldGrid; set => _fieldGrid = value; }
        #endregion
        public override Zones ZoneType => Zones.PLAY;

        public override float MoveCardTime => _settings.MoveCardToFieldTime;

        #region Methods
        protected void Awake()
        {
            CardList = new List<Card>();
            FieldGrid = new List<Row>();

            foreach (Transform rowT in transform)
            {
                Row row = new();
                FieldGrid.Add(row);
                foreach (Transform cellT in rowT.transform)
                {
                    Cell cell = cellT.GetComponent<Cell>();
                    row.cells.Add(cell);
                    cell.FieldPosition = new Vector2Int(row.cells.IndexOf(cell), FieldGrid.IndexOf(row));
                }
            }
        }

        public void AddToField(Card card, Cell cell)
        {
            CardList.Add(card);
            card.Zone = Zones.PLAY;
            cell.AddCard(card);
        }

        public void AddToField(Card card, Vector2Int fieldPos)
        {
            AddToField(card, FieldGrid[fieldPos.y].cells[fieldPos.x]);
        }

        public void RemoveCard(Card card)
        {
            CardList.Remove(card);
            if (card.Cell != null)
            {
                card.Cell.RemoveCard(card);
            }
        }

        public IEnumerator StartCombat()
        {
            Card[] activeCards = CardList.Where(s => s.Controller == _gameManager.ActivePlayer).Reverse().ToArray();
            for (int i = activeCards.Length - 1; i >= 0; i--)
            {

                yield return activeCards[i].Combat();
            }
        }

        public IEnumerator MoveCardAndFight(Card card, Vector2Int movement)
        {
            Vector2Int destination = new(
                Mathf.Clamp(card.FieldPosition.x + movement.x, 0, FieldGrid[0].cells.Count - 1),
                Mathf.Clamp(card.FieldPosition.y + movement.y, 0, FieldGrid.Count - 1));

            Card collided = null;
            IEnumerable<Card> activeCards = Enumerable.Empty<Card>();
            for (int i = card.FieldPosition.x + Math.Sign(movement.x); i != destination.x + Math.Sign(movement.x); i += Math.Sign(movement.x))
            {
                activeCards = FieldGrid[card.FieldPosition.y].cells[i].Cards.Where(x => x.isActiveAndEnabled);
                if (activeCards.Count() > 0)
                {
                    destination.x = i - Math.Sign(movement.x);
                    collided = FieldGrid[card.FieldPosition.y].cells[i].Cards[0];
                    break;
                }
            }

            if (destination != card.FieldPosition)
            {
                yield return _gameManager.MoveCardFromCellToCell(FieldGrid[card.FieldPosition.y].cells[card.FieldPosition.x],
                                                                  FieldGrid[destination.y].cells[destination.x]);
                if (FieldGrid[destination.y].cells[destination.x].Player != card.Controller && FieldGrid[destination.y].cells[destination.x].Player != Player.Neutral)
                {
                    yield return card.PassHomeRow();
                }
            }
            if (collided != null)
            {
                yield return card.Fight(collided);
            }
        }

        public IEnumerator MoveCardToCell(Card card, Cell cell)
        {
            yield return card.RectTransformMover.MoveToFitRectangle(cell.RectTransform, 0.2f);
        }

        public IEnumerator MoveCardToCell(Card card, Vector2Int fieldPos)
        {
            yield return MoveCardToCell(card, FieldGrid[fieldPos.y].cells[fieldPos.x]);
        }
        #endregion
    }
}
