using Abraxas.Cards.Controllers;
using Abraxas.Cells.Controllers;
using Abraxas.Events;
using Abraxas.Players.Managers;
using Abraxas.Zones.Controllers;
using Abraxas.Zones.Fields.Models;
using Abraxas.Zones.Fields.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Fields.Controllers
{
    class FieldController : ZoneController, IFieldController
    {
        #region Dependencies
        readonly IPlayerManager _playerManager;

        public FieldController(IPlayerManager playerManager)
        {
            _playerManager = playerManager;
        }
        #endregion
        #region Methods
        public IEnumerator StartCombat()
        {
            ICardController[] activeCards = Model.GetCardsForPlayer(_playerManager.ActivePlayer).ToArray();
            for (int i = activeCards.Length - 1; i >= 0; i--)
            {
                yield return activeCards[i].Combat();
            }
        }

        public IEnumerator CombatMovement(ICardController card, Point movement)
        {
            if (card.Zone != this) yield break;
            Point destination = new(
                Math.Clamp(card.Cell.FieldPosition.X + movement.X, 0, ((IFieldModel)Model).FieldGrid[0].Count - 1),
                Math.Clamp(card.Cell.FieldPosition.Y + movement.Y, 0, ((IFieldModel)Model).FieldGrid.Count - 1));

            ICardController collided = null;
            var FieldGrid = ((IFieldModel)Model).FieldGrid;

            for (int i = card.Cell.FieldPosition.X + Math.Sign(movement.X); i != destination.X + Math.Sign(movement.X); i += Math.Sign(movement.X))
            {
                if (FieldGrid[card.Cell.FieldPosition.Y][i].CardsOnCell <= 0) continue;
                destination.X = i - Math.Sign(movement.X);
                collided = FieldGrid[card.Cell.FieldPosition.Y][i].GetCardAtIndex(0);
                break;
            }

            if (destination != card.Cell.FieldPosition)
            {
                yield return MoveCardToCell(card, FieldGrid[destination.Y][destination.X]);
            }
            if (collided != null)
            {
                yield return card.Fight(collided);
            }
            else if (FieldGrid[destination.Y][destination.X].Player != card.Owner && FieldGrid[destination.Y][destination.X].Player != Player.Neutral)
            {
                yield return card.PassHomeRow();
            }
            else if (card.StatBlock.Stats.RNG > 0)
            {
                yield return CheckRangedAttack(card, movement, FieldGrid);
            }

        }

        private IEnumerator CheckRangedAttack(ICardController card, Point movement, List<List<ICellController>> FieldGrid)
        {
            if (card.Zone != this) yield break;
            Point destination = new(
                            Math.Clamp(card.Cell.FieldPosition.X + (card.StatBlock.Stats.RNG * Math.Sign(movement.X)), 0, ((IFieldModel)Model).FieldGrid[0].Count - 1),
                            card.Cell.FieldPosition.Y);
            ICardController collided = null;

            for (int i = card.Cell.FieldPosition.X + Math.Sign(movement.X); i != destination.X + Math.Sign(movement.X); i += Math.Sign(movement.X))
            {
                if (i > FieldGrid[0].Count - 1 || i < 0) break;
                if (FieldGrid[card.Cell.FieldPosition.Y][i].CardsOnCell <= 0) continue;
                destination.X = i - Math.Sign(movement.X);
                collided = FieldGrid[card.Cell.FieldPosition.Y][i].GetCardAtIndex(0);
                break;
            }
            if (collided != null)
            {
                yield return card.RangedAttack(collided);
            }
        }

        private IEnumerator MoveCardToCell(ICardController card, ICellController cell)
        {
            card.Cell?.RemoveCard(card);
            yield return ((IFieldView)View).MoveCardToCell(card, cell);
            cell.AddCard(card);

        }

        public IEnumerator MoveCardToCell(ICardController card, Point fieldPos)
        {
            yield return MoveCardToCell(card, ((IFieldModel)Model).FieldGrid[fieldPos.Y][fieldPos.X]);
        }

        public override void RemoveCard(ICardController card)
        {
            card.Cell?.RemoveCard(card);
            Model.RemoveCard(card);
        }

        public override void AddCard(ICardController card, int index = 0)
        {
            card.Hidden = false;
            base.AddCard(card, index);
        }

        public PointF GetDefaultCellDimensions()
        {
            return ((IFieldView)View).GetCellDimensions(((IFieldModel)Model).FieldGrid[0][0]);
        }

        public override IEnumerator OnEventRaised(Event_LocalPlayerChanged eventData)
        {
            ((IFieldModel)Model).GenerateField();
            return base.OnEventRaised(eventData);
        }

        public void HighlightPlayableOpenCells(ICardController cardController)
        {
            foreach (var cell in ((IFieldModel)Model).FieldGrid.SelectMany(x => x))
            {
                cell.HighlightPlayableOpenCell(cardController);
            }
        }

        public void SetHighlightVisible(bool val)
        {
            foreach (var cell in ((IFieldModel)Model).FieldGrid.SelectMany(x => x))
            {
                cell.SetHighlightVisible(val);
            }
        }

        public ICellController[] GetOpenCells(Player player)
        {
            var openCells = new List<ICellController>();
            foreach (var cell in ((IFieldModel)Model).FieldGrid.SelectMany(x => x))
            {
                if (cell.IsOpen(player))
                {
                    openCells.Add(cell);
                }
            }
            return openCells.ToArray();
        }
        #endregion
    }
}
