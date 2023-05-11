using Abraxas.Cards.Controllers;
using Abraxas.Players;
using Abraxas.Zones.Overlays;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using Zenject;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Fields
{
    public class FieldView : ZoneView
    {
        #region Settings
        Settings _settings;
        [Serializable]
        public class Settings
        {
            public float MoveCardToFieldTime;
            public float MoveCardOnFieldTime;
        }
        #endregion

        #region Dependencies
        IOverlayManager _overlayManager;
        IPlayerManager _playerManager;
        [Inject]
        public void Construct(Settings settings, IOverlayManager gameManager, IPlayerManager playerManager)
        {
            _settings = settings;
            _overlayManager = gameManager;
            _playerManager = playerManager;
        }
        #endregion

        #region Fields
        List<ICardController> _cardList = new();
        List<List<ICellView>> _fieldGrid = new();
        #endregion

        #region Properties
        public List<ICardController> CardList { get => _cardList; set => _cardList = value; }
        List<List<ICellView>> FieldGrid { get => _fieldGrid; set => _fieldGrid = value; }
        #endregion
        public override Zones ZoneType => Zones.PLAY;

        public override float MoveCardTime => _settings.MoveCardToFieldTime;

        #region Methods
        protected void Awake()
        {
            foreach (Transform rowT in transform)
            {
                List<ICellView> row = new();
                FieldGrid.Add(row);
                foreach (Transform cellT in rowT.transform)
                {
                    ICellView cell = cellT.GetComponent<ICellView>();
                    row.Add(cell);
                    cell.Controller.FieldPosition = new Point(FieldGrid[FieldGrid.IndexOf(row)].IndexOf(cell), FieldGrid.IndexOf(row));
                }
            }
        }

        internal PointF GetCellDimensions()
        {
            Vector2 dimensions = FieldGrid[0][0].RectTransform.rect.size;
            return new PointF(dimensions.x, dimensions.y);
        }

        public void AddToField(ICardController card, ICellController cell)
        {
            CardList.Add(card);
            card.Zone = Zones.PLAY;
        }

        public void AddToField(ICardController card, Point fieldPos)
        {
            AddToField(card, FieldGrid[fieldPos.Y][fieldPos.X].Controller);
        }

        public void RemoveCard(ICardController card)
        {
            CardList.Remove(card);
            card.Cell?.RemoveCard(card);
        }

        public IEnumerator StartCombat()
        {
            ICardController[] activeCards = CardList.Where(s => s.Owner == _playerManager.ActivePlayer).Reverse().ToArray();
            for (int i = activeCards.Length - 1; i >= 0; i--)
            {
                yield return activeCards[i].Combat();
            }
        }

        public IEnumerator MoveCardAndFight(ICardController card, Point movement)
        {
            Point destination = new(
                Mathf.Clamp(card.FieldPosition.X + movement.X, 0, FieldGrid[0].Count - 1),
                Mathf.Clamp(card.FieldPosition.Y + movement.Y, 0, FieldGrid.Count - 1));

            ICardController collided = null;
            IEnumerable<ICardController> activeCards;
            for (int i = card.FieldPosition.X + Math.Sign(movement.X); i != destination.X + Math.Sign(movement.X); i += Math.Sign(movement.X))
            {
                activeCards = FieldGrid[card.FieldPosition.Y][i].Controller.Cards;
                if (activeCards.Count() > 0)
                {
                    destination.X = i - Math.Sign(movement.X);
                    collided = FieldGrid[card.FieldPosition.Y][i].Controller.Cards[0];
                    break;
                }
            }

            if (destination != card.FieldPosition)
            {
                yield return MoveCardToCell(card, FieldGrid[destination.Y][destination.X].Controller);
                if (FieldGrid[destination.Y][destination.Y].Controller.Player != card.Owner && FieldGrid[destination.Y][destination.X].Controller.Player != Player.Neutral)
                {
                    yield return card.PassHomeRow();
                }
            }
            if (collided != null)
            {
                yield return card.Fight(collided);
            }
        }

        public IEnumerator MoveCardToCell(ICardController card, ICellController cell)
        {
            card.Cell?.RemoveCard(card);
            _overlayManager.AddCard(card);
            yield return card.View.RectTransformMover.MoveToFitRectangle(cell.View.RectTransform, MoveCardTime);
            cell.AddCard(card);
            _overlayManager.RemoveCard(card);
        }

        public IEnumerator MoveCardToCell(ICardController card, Point fieldPos)
        {
            yield return MoveCardToCell(card, FieldGrid[fieldPos.Y][fieldPos.X].Controller);
        }
        #endregion
    }
}
