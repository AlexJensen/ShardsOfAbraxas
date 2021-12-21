using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDetail : Singleton<CardDetail>
{
    public enum ScreenSide
    {
        LEFT,
        RIGHT
    }

    public RectTransform cardDetail;
    public Text title, cost;


    [Serializable]
    public struct Side
    {
        public ScreenSide side;
        public RectTransform rectTranform;
    }

    public List<Side> sides;

    public void ShowCardDetailOnSide(Card card, ScreenSide side)
    {
        cardDetail.gameObject.SetActive(true);
        Side screenSide = sides.Find(x => x.side == side);
        cardDetail.SetParent(screenSide.rectTranform.transform);
        cardDetail.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, screenSide.rectTranform.rect.width);
        cardDetail.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, screenSide.rectTranform.rect.height);
    }

    public void HideCardDetail()
    {
        cardDetail.gameObject.SetActive(false);
    }
}
