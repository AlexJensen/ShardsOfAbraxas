using Abraxas.Core;
using Abraxas.Behaviours.Stones;
using Abraxas.Behaviours.Data;
using Abraxas.Behaviours.Game;
using Abraxas.Behaviours.Players;
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
using Zenject;

namespace Abraxas.Behaviours.Cards
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(StatBlock))]
    public class Card : NetworkBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        #region Dependency Injections
        GameManager _gameManager;
        DragManager _dragManager;
        DataManager _dataManager;
        CardViewerManager _cardViewerManager;
        FieldManager _fieldManager;

        [Inject]
        public void Construct(GameManager gameManager, DragManager dragManager, DataManager dataManager, CardViewerManager cardViewerManager, FieldManager fieldManager)
        {
            _gameManager = gameManager;
            _dragManager = dragManager;
            _dataManager = dataManager;
            _cardViewerManager = cardViewerManager;
            _fieldManager = fieldManager;
        }
        #endregion

        #region Constants
        public const float MOVE_FROM_HAND_TO_DRAG_SCALE_TIME = .3f;
        public const float MOVEMENT_ON_FIELD_TIME = .3f;
        public const float MOVE_TO_ZONE_SCALE_TIME = .3f;
        public const float MOVE_TO_ZONE_MOVE_TIME = .3f;
        public const float MOVE_FROM_HAND_TO_CELL_SCALE_TIME = 0.3f;
        public const float MOVE_FROM_HAND_TO_CELL_MOVE_TIME = 0.3f;
        #endregion

        #region Fields
        [SerializeField]
        string _title = "";

        [SerializeField]
        Player _controller, _owner;

        [SerializeField]
        GameObject _cover;

        [SerializeField]
        bool _hidden = false;

        [SerializeField]
        TMP_Text _titleText, _costText;

        [SerializeField]
        Image _image;

        Vector2Int _fieldPosition;
        Dictionary<StoneData.StoneType, int> _totalCosts;
        string _totalCostText;
        ZoneManager.Zones _zone;
        List<Stone> _stones;
        StatBlock _statBlock;
        Canvas _canvas;
        GraphicRaycaster _graphicRaycaster;
        #endregion

        #region Properties
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                _titleText.text = _title;
            }
        }
        public Player Controller
        {
            get => _controller;
            set
            {
                _controller = value;
                _image.transform.localScale = new Vector3(Owner == Player.Player1 ? 1 : -1, 1, 1);
            }
        }
        public Player Owner
        {
            get => _owner;
            private set
            {
                _owner = value;
                Color controllerColor = _dataManager.GetPlayerDetails(Controller).color;
                _titleText.color = controllerColor;
            }
        }

        public string TotalCostText
        {
            get => _totalCostText; 
            private set
            {
                _totalCostText = value;
                _costText.text = _totalCostText;
            }
        }

        public RectTransform RectTransform => (RectTransform)transform;
        public List<Stone> Stones => _stones ??= GetComponents<Stone>().ToList();
        public StatBlock StatBlock => _statBlock = _statBlock != null ? _statBlock : GetComponent<StatBlock>();
        public Image Image => _image;
        public Canvas Canvas => _canvas = _canvas != null ? _canvas : GetComponentInParent<Canvas>();
        public GraphicRaycaster GraphicRaycaster => _graphicRaycaster = _graphicRaycaster != null ? _graphicRaycaster : Canvas.GetComponent<GraphicRaycaster>();
        
        public Dictionary<StoneData.StoneType, int> TotalCosts
        {
            get => _totalCosts ??= GenerateTotalCost(); 
            private set => _totalCosts = value; 
        }

        public Cell Cell { get; set; }
        public Vector2Int FieldPosition { get => _fieldPosition; set => _fieldPosition = value; }
        public ZoneManager.Zones Zone { get => _zone; set => _zone = value; }
        public bool Hidden { get => _hidden; set => _hidden = value; }
        #endregion

        #region Unity Methods
        void Awake()
        {
            Controller = _controller;
            Owner = _owner;
        }
        #endregion

        #region Methods
        private Dictionary<StoneData.StoneType, int> GenerateTotalCost()
        {
            _totalCosts = new Dictionary<StoneData.StoneType, int>();
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
            return _totalCosts;
        }

        private void FormatCost()
        {
            string TotalCost = "";
            foreach (KeyValuePair<StoneData.StoneType, int> pair in TotalCosts)
            {
                if (pair.Value != 0)
                {
                    TotalCost += "<#" + ColorUtility.ToHtmlStringRGB(_dataManager.GetStoneDetails(pair.Key).color) + ">" + pair.Value;
                }
            }
            TotalCostText = TotalCost;
        }

        public IEnumerator PassHomeRow()
        {
            _gameManager.DamagePlayer(Controller ==
                Player.Player1 ? Player.Player2 : Player.Player1,
                StatBlock[StatBlock.StatValues.ATK]);
            yield return _gameManager.GetPlayerHand(Controller).Deck.MoveCardToZone(this);
        }

        public IEnumerator Fight(Card collided)
        {
            if (collided.Controller == Controller)
            {
                yield return null;
            }
            StatBlock collidedStats = collided.GetComponent<StatBlock>();
            collidedStats[StatBlock.StatValues.DEF] -= StatBlock[StatBlock.StatValues.ATK];
            StatBlock[StatBlock.StatValues.DEF] -= collidedStats[StatBlock.StatValues.ATK];


            yield return Utilities.WaitForCoroutines(this,
                collided.CheckDeath(),
                CheckDeath());
        }

        private IEnumerator CheckDeath()
        {
            if (StatBlock[StatBlock.StatValues.DEF] <= 0)
            {
                yield return _gameManager.GetPlayerHand(Controller).Graveyard.MoveCardToZone(this);
            }
        }
        #endregion

        #region Input Events
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!Hidden)
            {
                switch (Zone)
                {
                    case ZoneManager.Zones.HAND:
                        {
                            if (Controller == _gameManager.ActivePlayer)
                            {
                                transform.SetParent(_dragManager.transform);
                                _dragManager.card = this;
                                _gameManager.GetPlayerHand(Controller).RemoveCard(this);
                                StartCoroutine(ChangeScale(_dragManager.templateCardRect.rect.size, MOVE_FROM_HAND_TO_DRAG_SCALE_TIME));
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
            _dragManager.card = null;
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
                                if (cell.Cards.Count == 0 && cell.Player == Controller && _gameManager.CanPurchaseCard(this))
                                {

                                    _gameManager.PurchaseCard(this);
                                    gameObject.AddComponent<LoadingDebuff>();
                                    RequestMoveToCellServerRpc(cell.FieldPosition);
                                    return;
                                }
                            }
                        }
                        StartCoroutine(_gameManager.GetPlayerHand(Controller).MoveCardToZone(this));
                    }
                    break;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Input.mousePosition.x > Screen.width / 2)
            {
                _cardViewerManager.ShowCardDetailOnSide(this, CardViewerManager.ScreenSide.LEFT);
            }
            else
            {
                _cardViewerManager.ShowCardDetailOnSide(this, CardViewerManager.ScreenSide.RIGHT);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _cardViewerManager.HideCardDetail();
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

        public IEnumerator MoveToFitRectangle(RectTransform rectTransform)
        {
            transform.SetParent(_dragManager.transform);
            yield return StartCoroutine(Utilities.WaitForCoroutines(this,
                ChangeScale(rectTransform.rect.size, MOVE_TO_ZONE_SCALE_TIME),
                MoveTo(rectTransform.position, MOVE_TO_ZONE_MOVE_TIME)));
        }

        public IEnumerator MoveToCell(Cell cell)
        {
            transform.SetParent(_dragManager.transform);
            _gameManager.GetPlayerHand(Controller).RemoveCard(this);
            yield return StartCoroutine(Utilities.WaitForCoroutines(this,
               ChangeScale(cell.RectTransform.rect.size, MOVE_FROM_HAND_TO_CELL_SCALE_TIME),
               MoveTo(cell.RectTransform.position, MOVE_FROM_HAND_TO_CELL_MOVE_TIME)));
            _fieldManager.AddToField(this, cell);
        }

        public IEnumerator OnCombat()
        {
            if (gameObject.GetComponent<LoadingDebuff>() == null)
            {
                yield return StartCoroutine(_fieldManager.MoveCardAndFight(this, new Vector2Int(
                  Controller == Player.Player1 ? StatBlock[StatBlock.StatValues.MV] :
                  Controller == Player.Player2 ? -StatBlock[StatBlock.StatValues.MV] : 0, 0)));
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
            StartCoroutine(_fieldManager.MoveToFieldPosition(this, fieldPos));
        }
        #endregion
    }
}