using Abraxas.Cards.Controllers;
using Abraxas.Cells.Controllers;
using Abraxas.Cells.Views;
using Abraxas.Players.Managers;
using Abraxas.Zones.Views;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Netcode;
using UnityEngine;
using Zenject;
using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Fields.Views
{
    public class FieldView : ZoneView, IFieldView
    {
        #region Dependencies

        IPlayerManager _playerManager;
        [Inject]
        public void Construct(IPlayerManager playerManager)
        {
            _playerManager = playerManager;
        }
        #endregion

        #region Properties
        protected override float MoveCardTime => NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsHost ? 0 : AnimationSettings.MoveCardToFieldTime;
        #endregion

        #region Methods
        public List<List<ICellController>> GenerateField()
        {
            List<List<ICellController>> field = new();

            for (int rowIndex = 0; rowIndex < CardHolder.childCount; rowIndex++)
            {
                Transform row = CardHolder.GetChild(rowIndex);
                List<ICellController> newRow = new();
                List<Transform> cells = new();

                // Collect all cells first
                for (int cellIndex = 0; cellIndex < row.childCount; cellIndex++)
                {
                    Transform cell = row.GetChild(cellIndex);
                    var cellController = cell.GetComponent<ICellView>().Controller;

                    // Assign the FieldPosition based on row and cell index
                    cellController.FieldPosition = new Point(cellIndex, rowIndex);

                    cells.Add(cell);
                    newRow.Add(cellController);
                }

                // If local player is Player 2, reverse the sibling order
                if (_playerManager.LocalPlayer == Player.Player2)
                {
                    cells.Reverse(); // Reverse the list of cells

                    // Apply the reversed sibling order
                    for (int i = 0; i < cells.Count; i++)
                    {
                        cells[i].SetSiblingIndex(i);
                    }
                }

                field.Add(newRow);
            }

            return field;
        }

        public PointF GetCellDimensions(ICellController cell)
        {
            Vector2 dimensions = cell.RectTransform.rect.size;
            return new PointF(dimensions.x, dimensions.y);
        }

        public IEnumerator MoveCardToCell(ICardController card, ICellController cell)
        {
            OverlayManager.SetCard(card);
            yield return card.MoveToCell(cell, MoveCardTime);
            OverlayManager.ClearCard(card);
        }

        public override void AddCardToHolder(ICardController card, int index = 0)
        {
            //
        }
        #endregion
    }
}
