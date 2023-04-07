using Abraxas.Core;
using Abraxas.Behaviours.Stones;
using Abraxas.Behaviours.Data;
using Abraxas.Behaviours.Game;
using Abraxas.Behaviours.Zones;
using Abraxas.Behaviours.Zones.Fields;
using Abraxas.Behaviours.Zones.Drags;
using Abraxas.Behaviours.Status;
using Abraxas.Behaviours.CardViewer;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Abraxas.Behaviours.Cards
{
    /// <summary>
    /// Represents a Card object, stores information about the current state of the card, and includes methods to move and scale the card.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(StatBlock))]
    public class Card : NetworkBehaviour, ICardInputHandler
    {
        #region Constants
        public const float MOVE_FROM_HAND_TO_DRAG_SCALE_TIME = .2f;
        public const float MOVEMENT_ON_FIELD_TIME = .3f;
        public const float MOVE_TO_ZONE_SCALE_TIME = .3f;
        public const float MOVE_TO_ZONE_MOVE_TIME = .3f;
        public const float MOVE_FROM_HAND_TO_CELL_SCALE_TIME = 0.2f;
        public const float MOVE_FROM_HAND_TO_CELL_MOVE_TIME = 0.2f;
        #endregion

        #region Fields
        [SerializeField]
        string title = "";

        [SerializeField]
        GameManager.Player controller, owner;

        [SerializeField]
        GameObject cover;

        [SerializeField]
        bool hidden = false;

        [SerializeField]
        TMP_Text titleText, costText;

        [SerializeField]
        Image image;

        Vector2Int fieldPosition;
        Dictionary<StoneData.StoneType, int> totalCosts;
        string totalCostText;
        ZoneManager.Zones zone;
        RectTransform rectTransform;
        List<Stone> stones;
        StatBlock statBlock;
        Canvas canvas;
        GraphicRaycaster graphicRaycaster;
        #endregion

        #region Properties
        public string Title
        {
            get => title;
            set
            {
                title = value;
                titleText.text = title;
            }
        }
        public GameManager.Player Controller
        {
            get => controller;
            set
            {
                controller = value;
                image.transform.localScale = new Vector3(owner == GameManager.Player.Player1 ? 1 : -1, 1, 1);
            }
        }
        public GameManager.Player Owner
        {
            get => owner;
            set
            {
                owner = value;
                Color controllerColor = DataManager.Instance.GetPlayerDetails(controller).color;
                titleText.color = controllerColor;
            }
        }

        public string TotalCostText
        {
            get => totalCostText; private set
            {
                totalCostText = value;
                costText.text = totalCostText;
            }
        }

        public RectTransform RectTransform => rectTransform = rectTransform != null ? rectTransform : (RectTransform)transform;
        public List<Stone> Stones => stones ??= GetComponents<Stone>().ToList();
        public StatBlock StatBlock => statBlock ??= GetComponent<StatBlock>();
        public Image Image => image;
        public Canvas Canvas => canvas ??= GetComponentInParent<Canvas>();
        public GraphicRaycaster GraphicRaycaster => graphicRaycaster ??= Canvas.GetComponent<GraphicRaycaster>();
        
        public Dictionary<StoneData.StoneType, int> TotalCosts { get => totalCosts ??= GenerateTotalCost(); }
        public Cell Cell { get; internal set; }
        public Vector2Int FieldPosition { get => fieldPosition; set => fieldPosition = value; }
        public ZoneManager.Zones Zone { get => zone; set => zone = value; }
        #endregion

        #region Unity Methods
        void Awake()
        {
            Controller = controller;
            Owner = owner;
        }

        void Update()
        {
            if (hidden != cover.activeInHierarchy) cover.SetActive(hidden);
        }

        private Dictionary<StoneData.StoneType, int> GenerateTotalCost()
        {
            totalCosts = new Dictionary<StoneData.StoneType, int>();
            foreach (Stone stone in Stones)
            {
                stone.card = this;
                if (!TotalCosts.ContainsKey(stone.StoneType))
                {
                    TotalCosts.Add(stone.StoneType, stone.Cost);
                }
                else
                {
                    TotalCosts[stone.StoneType] += stone.Cost;
                }
            }
            FormatCost();
            return totalCosts;
        }



        internal IEnumerator PassHomeRow()
        {
            GameManager.Instance.DamagePlayer(Controller ==
                GameManager.Player.Player1 ? GameManager.Player.Player2 : GameManager.Player.Player1,
                StatBlock[StatBlock.StatValues.ATK]);
            yield return GameManager.Instance.GetPlayerHand(Controller).Deck.MoveCardToZone(this);
        }

        public IEnumerator Fight(Card collided)
        {
            if (collided.controller != controller)
            {
                StatBlock collidedStats = collided.GetComponent<StatBlock>();
                collidedStats[StatBlock.StatValues.DEF] -= StatBlock[StatBlock.StatValues.ATK];
                StatBlock[StatBlock.StatValues.DEF] -= collidedStats[StatBlock.StatValues.ATK];


                yield return Utilities.WaitForCoroutines(this,
                    collided.CheckDeath(),
                    CheckDeath());
            }
        }

        private IEnumerator CheckDeath()
        {
            if (statBlock[StatBlock.StatValues.DEF] <= 0)
            {
                yield return GameManager.Instance.GetPlayerHand(Controller).Graveyard.MoveCardToZone(this);
            }
        }

        private void FormatCost()
        {
            string TotalCost = "";
            foreach (KeyValuePair<StoneData.StoneType, int> pair in TotalCosts)
            {
                if (pair.Value != 0)
                {
                    TotalCost += "<#" + ColorUtility.ToHtmlStringRGB(DataManager.Instance.GetStoneDetails(pair.Key).color) + ">" + pair.Value;
                }
            }
            TotalCostText = TotalCost;
        }
        #endregion

        #region Input Events
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!hidden)
            {
                switch (Zone)
                {
                    case ZoneManager.Zones.HAND:
                        {
                            if (Controller == GameManager.Instance.ActivePlayer)
                            {
                                transform.SetParent(DragManager.Instance.transform);
                                DragManager.Instance.card = this;
                                GameManager.Instance.GetPlayerHand(Controller).RemoveCard(this);
                                StartCoroutine(ChangeScale(DragManager.Instance.templateCardRect.rect.size, MOVE_FROM_HAND_TO_DRAG_SCALE_TIME));
                                Zone = ZoneManager.Zones.DRAG;
                            }
                        }
                        break;
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            switch (Zone)
            {
                case ZoneManager.Zones.DRAG:
                    {
                        RectTransform.position = Input.mousePosition;
                    }
                    break;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            DragManager.Instance.card = null;
            switch (Zone)
            {
                case ZoneManager.Zones.DRAG:
                    {
                        List<RaycastResult> results = new();
                        GraphicRaycaster.Raycast(eventData, results);
                        foreach (var hit in results)
                        {
                            Cell cell = hit.gameObject.GetComponent<Cell>();
                            if (cell)
                            {
                                if (cell.Cards.Count == 0 && cell.player == Controller && GameManager.Instance.CanPurchaseCard(this))
                                {
                                    
                                    GameManager.Instance.PurchaseCard(this);
                                    gameObject.AddComponent<LoadingDebuff>();
                                    RequestMoveToCellServerRpc(cell.fieldPos);
                                    return;
                                }
                            }
                        }
                        StartCoroutine(GameManager.Instance.GetPlayerHand(Controller).MoveCardToZone(this));
                    }
                    break;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Input.mousePosition.x > Screen.width / 2)
            {
                CardViewerManager.Instance.ShowCardDetailOnSide(this, CardViewerManager.ScreenSide.LEFT);
            }
            else
            {
                CardViewerManager.Instance.ShowCardDetailOnSide(this, CardViewerManager.ScreenSide.RIGHT);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CardViewerManager.Instance.HideCardDetail();
        }
        #endregion

        #region Animation Lerps
        private IEnumerator ChangeScale(Vector2 size, float time)
        {
            Vector2 origSize = RectTransform.rect.size;
            float lerpIncrement = 1 / time, lerpProgress = 0;
            while (lerpProgress <= 1)
            {
                Vector2 lerpUpdate = Vector2.Lerp(origSize, size, lerpProgress);
                RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, lerpUpdate.x);
                RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, lerpUpdate.y);

                lerpProgress += lerpIncrement * Time.fixedDeltaTime;
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

                lerpProgress += lerpIncrement * Time.fixedDeltaTime;
                yield return null;
            }
            yield return null;
        }

        public IEnumerator MoveToFitRectangle(RectTransform rectTransform)
        {
            transform.SetParent(DragManager.Instance.transform);
            yield return StartCoroutine(Utilities.WaitForCoroutines(this,
                ChangeScale(rectTransform.rect.size, MOVE_TO_ZONE_SCALE_TIME),
                MoveTo(rectTransform.position, MOVE_TO_ZONE_MOVE_TIME)));
        }

        public IEnumerator MoveToCell(Cell cell)
        {
            transform.SetParent(DragManager.Instance.transform);
            GameManager.Instance.GetPlayerHand(Controller).RemoveCard(this);
            yield return StartCoroutine(Utilities.WaitForCoroutines(this,
               ChangeScale(cell.RectTransform.rect.size, MOVE_FROM_HAND_TO_CELL_SCALE_TIME),
               MoveTo(cell.RectTransform.position, MOVE_FROM_HAND_TO_CELL_MOVE_TIME)));
            FieldManager.Instance.AddToField(this, cell);
        }

        public IEnumerator OnCombat()
        {
            if (gameObject.GetComponent<LoadingDebuff>() == null)
            {
                yield return StartCoroutine(FieldManager.Instance.MoveCardAndFight(this, new Vector2Int(
                  Controller == GameManager.Player.Player1 ? statBlock[StatBlock.StatValues.MV] :
                  Controller == GameManager.Player.Player2 ? -statBlock[StatBlock.StatValues.MV] : 0, 0)));
            }
        }
        #endregion

        #region Server Methods
        [ServerRpc(RequireOwnership = false)]
        void RequestMoveToCellServerRpc(Vector2Int fieldPos)
        {
            if (!IsServer) return;
            MoveToCellClientRpc(fieldPos);
        }

        [ClientRpc]
        void MoveToCellClientRpc(Vector2Int fieldPos)
        {
            if (!IsClient) return;
            StartCoroutine(FieldManager.Instance.MoveToFieldPosition(this, fieldPos));
        }
        #endregion
    }
}