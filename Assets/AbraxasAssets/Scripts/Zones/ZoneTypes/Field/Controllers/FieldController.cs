using Abraxas.Cards.Controllers;
using Abraxas.Cells.Controllers;
using Abraxas.Events;
using Abraxas.Players.Managers;
using Abraxas.Zones.Controllers;
using Abraxas.Zones.Fields.Models;
using Abraxas.Zones.Fields.Views;
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

        #region Properties
        public List<List<ICellController>> FieldGrid { get { return ((IFieldModel)Model).FieldGrid; } }
        #endregion
        #region Methods
        public IEnumerator StartCombat()
        {
            ICardController[] activeCards = Model.GetCardsForPlayer(_playerManager.ActivePlayer).ToArray();
            for (int i = activeCards.Length - 1; i >= 0; i--)
            {
                yield return activeCards[i].PreCombat();
            }
            for (int i = activeCards.Length - 1; i >= 0; i--)
            {
                yield return activeCards[i].Combat(this);
            }
        }

        public IEnumerator MoveCardToCell(ICardController card, ICellController cell)
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
