using Abraxas.Cards;
using Abraxas.Cards.Controllers;
using Abraxas.Cards.Views;
using Abraxas.Cells.Controllers;
using Abraxas.Cells.Views;
using Abraxas.Zones.Overlays;
using Abraxas.Zones.Views;
using System.Collections;
using System.Drawing;
using UnityEngine;
using Zenject;

namespace Abraxas.Zones.Fields.Views
{
    public class FieldView : ZoneView, IFieldView
    {
        #region Dependencies
        IOverlayManager _overlayManager;
        Card.Settings.AnimationSettings _cardAnimationSettings;
        [Inject]
        public void Construct(Card.Settings cardSettings, IOverlayManager overlayManager)
        {
            _cardAnimationSettings = cardSettings.animationSettings;
            _overlayManager = overlayManager;
        }
        #endregion

        #region Properties
        public override float MoveCardTime => _cardAnimationSettings.MoveCardToFieldTime;
        #endregion

        #region Methods
        public PointF GetCellDimensions(ICellView cell)
        {
            Vector2 dimensions = cell.RectTransform.rect.size;
            return new PointF(dimensions.x, dimensions.y);
        }

        public IEnumerator MoveCardToCell(ICardView card, ICellView cell)
        {
            _overlayManager.SetCard(card);
            yield return card.MoveToCell(cell, MoveCardTime);
            _overlayManager.ClearCard(card);
        }
        #endregion
    }
}
