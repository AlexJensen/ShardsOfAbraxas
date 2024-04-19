using Abraxas.Cards.Controllers;
using Abraxas.Cells.Controllers;
using Abraxas.Cells.Views;
using Abraxas.Zones.Views;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Netcode;
using UnityEngine;

namespace Abraxas.Zones.Fields.Views
{
	public class FieldView : ZoneView, IFieldView
    {
        #region Properties
        public override float MoveCardTime => NetworkManager.Singleton.IsServer ? 0 : AnimationSettings.MoveCardToFieldTime;

        public List<List<ICellController>> GenerateField()
        {
            List<List<ICellController>> field = new();
            foreach (Transform row in CardHolder)
            {
                List<ICellController> newRow = new();
                foreach (Transform cell in row)
                {
                    newRow.Add(cell.GetComponent<ICellView>().Controller);
                }
                field.Add(newRow);
            }
            return field;
        }
        #endregion

        #region Methods
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
