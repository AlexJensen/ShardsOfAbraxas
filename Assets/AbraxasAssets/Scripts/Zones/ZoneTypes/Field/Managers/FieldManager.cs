using Abraxas.Cards.Controllers;
using Abraxas.Cells.Controllers;
using Abraxas.Cells.Views;
using Abraxas.Zones.Factories;
using Abraxas.Zones.Fields.Controllers;
using Abraxas.Zones.Fields.Models;
using Abraxas.Zones.Fields.Views;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Zenject;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Fields.Managers
{

    class FieldManager : MonoBehaviour, IFieldManager
    {
        #region Dependencies
        IFieldController _field;

        [Inject]
        public void Construct(ZoneFactory<IFieldView, FieldController, FieldModel> fieldFactory, CellController.Factory cellFactory)
        {
            foreach (var cellView in FindObjectsOfType<CellView>())
            {
                cellFactory.Create(cellView);
            }
            var fields = FindObjectOfType<FieldView>();
            if (fields == null) return;
            _field = fieldFactory.Create(FindObjectOfType<FieldView>());
        }
        #endregion

        #region Methods

        public void RemoveCard(ICardController card)
        {
            _field.RemoveCard(card);
        }
        public IEnumerator StartCombat()
        {
            yield return _field.StartCombat();
        }
        public IEnumerator MoveCardToCell(ICardController card, Point fieldPos)
        {
            yield return _field.MoveCardToCell(card, fieldPos);
        }

        public void AddCard(ICardController card)
        {
            _field.AddCard(card);
        }

        public PointF GetCellDimensions()
        {
            return _field.GetDefaultCellDimensions();
        }
        public void SetField(IFieldController fieldController)
        {
            _field = fieldController;
        }

        public void HighlightPlayableOpenCells(ICardController cardController)
        {
            _field.HighlightPlayableOpenCells(cardController);
        }

        public ICellController[] GetOpenCells(Player player)
        {
            return _field.GetOpenCells(player);
        }

        public void SetHighlightVisible(bool val)
        {
            _field.SetHighlightVisible(val);
        }

        public List<ICellController> GetRow(int y)
        {
            return _field.FieldGrid[y];
        }
        #endregion
    }
}