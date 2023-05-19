using Abraxas.Cards.Controllers;
using Abraxas.Zones.Fields.Views;
using Abraxas.Cells.Controllers;
using System;
using System.Collections;
using System.Linq;
using System.Drawing;

using Player = Abraxas.Players.Players;
using Abraxas.Zones.Controllers;
using Abraxas.Players.Managers;
using Abraxas.Zones.Fields.Models;
using Zenject;

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
        public class Factory : PlaceholderFactory<IFieldView, IFieldController, IFieldModel>
        {

        }
        #endregion


        #region Methods
        public IEnumerator StartCombat()
        {
            ICardController[] activeCards = Model.GetCardsForPlayer(_playerManager.ActivePlayer).Reverse().ToArray();
            for (int i = activeCards.Length - 1; i >= 0; i--)
            {
                yield return activeCards[i].Combat();
            }
        }

        public IEnumerator MoveCardAndFight(ICardController card, Point movement)
        {
            Point destination = new(
                Math.Clamp(card.FieldPosition.X + movement.X, 0, ((IFieldModel)Model).FieldGrid[0].Count - 1),
                Math.Clamp(card.FieldPosition.Y + movement.Y, 0, ((IFieldModel)Model).FieldGrid.Count - 1));

            ICardController collided = null;
            var FieldGrid = ((IFieldModel)Model).FieldGrid;

            for (int i = card.FieldPosition.X + Math.Sign(movement.X); i != destination.X + Math.Sign(movement.X); i += Math.Sign(movement.X))
            {
                if (FieldGrid[card.FieldPosition.Y][i].CardsOnCell <= 0) continue;
                destination.X = i - Math.Sign(movement.X);
                collided = FieldGrid[card.FieldPosition.Y][i].GetCardAtIndex(0);
                break;
            }
            
            if (destination != card.FieldPosition)
            {
                yield return MoveCardToCell(card, ((IFieldModel)Model).FieldGrid[destination.Y][destination.X]);
                if (FieldGrid[destination.Y][destination.Y].Player != card.Owner && FieldGrid[destination.Y][destination.X].Player != Player.Neutral)
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
            yield return ((IFieldView)View).MoveCardToCell(card.View, cell.View);
            cell.AddCard(card);
        }

        public IEnumerator MoveCardToCell(ICardController card, Point fieldPos)
        {
            yield return MoveCardToCell(card, ((IFieldModel)Model).FieldGrid[fieldPos.Y][fieldPos.X]);
        }

        public override ICardController RemoveCard(ICardController card)
        {
            return Model.RemoveCard(card);
        }

        public void AddCard(ICardController card, Point fieldPos)
        {
            Model.AddCard(card);
        }

        public PointF GetDefaultCellDimensions()
        {
            return ((IFieldView)View).GetCellDimensions(((IFieldModel)Model).FieldGrid[0][0].View);
        }
        #endregion
    }
}
