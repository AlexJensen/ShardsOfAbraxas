using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum Zone
{
    DECK, DRAG, HAND, PLAY, DEAD, BANISHED
}

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(StatBlock))]
public class Card : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public const float SCALE_TIME = .2f;
    public const float MOVE_TIME = .2f;

    [SerializeField]
    string title = "", cost;

    public Zone zone;
    public Vector2Int fieldPos;
    public GameData.Players controller, owner;

    RectTransform rectTransform;
    StatBlock statBlock;

    Canvas canvas;
    GraphicRaycaster graphicRaycaster;

    public RectTransform RectTransform { get => rectTransform; }

    #region Input Events
    public void OnBeginDrag(PointerEventData eventData)
    {
        switch (zone)
        {
            case Zone.HAND:
                {
                    transform.SetParent(Drag.Instance.transform);
                    Drag.Instance.card = this;
                    StartCoroutine(ChangeScale(Drag.Instance.templateCardRect.rect.size, SCALE_TIME));
                    zone = Zone.DRAG;
                }
                break;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        switch (zone)
        {
            case Zone.DRAG:
                {
                    RectTransform.position = Input.mousePosition;
                }
                break;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        switch (zone)
        {
            case Zone.DRAG:
                {
                    List<RaycastResult> results = new();
                    graphicRaycaster.Raycast(eventData, results);

                    foreach (var hit in results)
                    {
                        Cell cell = hit.gameObject.GetComponent<Cell>();
                        if (cell)
                        {
                            if (cell.Cards.Count == 0 && cell.player == controller)
                            {
                                StopCoroutine(nameof(ChangeScale));
                                Field.Instance.AddToField(this, cell);
                                Drag.Instance.card = null;
                                break;
                            }
                        }
                        CardPlaceholder placeholder = hit.gameObject.GetComponent<CardPlaceholder>();
                        if (placeholder)
                        {

                        }

                    }
                }
                break;
        }
    }
    #endregion

    #region Unity Methods

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
        statBlock = GetComponent<StatBlock>();
    }

    void Update()
    {
        if (title != "") name = title;
    }
    #endregion

    #region Animation Lerps
    public IEnumerator ChangeScale(Vector2 size, float time)
    {
        Vector2 origSize = RectTransform.rect.size;
        float lerpIncrement = 1 / time, lerpProgress = 0;
        while(lerpProgress <= 1)
        {
            Vector2 lerpUpdate = Vector2.Lerp(origSize, size, lerpProgress);
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, lerpUpdate.x);
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, lerpUpdate.y);

            lerpProgress += lerpIncrement * Time.deltaTime;
            yield return null;
        }
        yield return null;
    }

    public IEnumerator MoveTo(Vector2 location, float time)
    {
        Vector2 origLoc = RectTransform.position;
        float lerpIncrement = 1 / time, lerpProgress = 0;
        while (lerpProgress <= 1)
        {
            Vector2 lerpUpdate = Vector2.Lerp(origLoc, location, lerpProgress);
            RectTransform.position = lerpUpdate;

            lerpProgress += lerpIncrement * Time.deltaTime;
            yield return null;
        }
        yield return null;
    }
    #endregion

    public IEnumerator OnCombat()
    {
        yield return StartCoroutine(Field.Instance.MoveCard(this, new Vector2Int(controller == GameData.Players.PLAYER_1 ? statBlock.stats.MV:
                                                                                 controller == GameData.Players.PLAYER_2 ? -statBlock.stats.MV: 0, 0)));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.mousePosition.x > Screen.width / 2)
        {
            CardDetail.Instance.ShowCardDetailOnSide(this, CardDetail.ScreenSide.LEFT);
        }
        else
        {
            CardDetail.Instance.ShowCardDetailOnSide(this, CardDetail.ScreenSide.RIGHT);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CardDetail.Instance.HideCardDetail();
    }
}


