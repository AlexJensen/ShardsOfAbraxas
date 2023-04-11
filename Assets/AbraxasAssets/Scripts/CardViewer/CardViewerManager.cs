using Abraxas.Behaviours.Cards;
using Abraxas.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Abraxas.Behaviours.CardViewer
{

    /// <summary>
    /// Positions the Card Detail pane based on the mouse position and controls displaying and hiding it.
    /// </summary>
    public class CardViewerManager : Singleton<CardViewerManager>
    {
        #region Constants
        /// <summary>
        /// Side of screen to display on.
        /// </summary>
        public enum ScreenSide
        {
            LEFT,
            RIGHT
        }

        /// <summary>
        /// Joiner to connect enum value to location on canvas.
        /// </summary>
        [Serializable]
        public struct Side
        {
            public ScreenSide side;
            public RectTransform rectTranform;
        }
        #endregion

        #region Fields
        public CardViewer _cardDetail;
        RectTransform _cardDetailRect;
        public List<Side> _sides;
        #endregion

        #region Unity Methods
        protected void Awake()
        {
            _cardDetailRect = (RectTransform)_cardDetail.transform;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Displays the card detail pane on the specified side of the screen.
        /// </summary>
        /// <param name="card">Card info to display.</param>
        /// <param name="side">Side of screen to display on.</param>
        public void ShowCardDetailOnSide(Card card, ScreenSide side)
        {
            _cardDetail.gameObject.SetActive(true);
            Side screenSide = _sides.Find(x => x.side == side);
            _cardDetailRect.SetParent(screenSide.rectTranform.transform);
            _cardDetailRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, screenSide.rectTranform.rect.width);
            _cardDetailRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, screenSide.rectTranform.rect.height);
            _cardDetail.ShowCardDetails(card);
        }

        /// <summary>
        /// Hides the card detail pane.
        /// </summary>
        public void HideCardDetail()
        {
            _cardDetail.gameObject.SetActive(false);
        }
        #endregion
    }
}