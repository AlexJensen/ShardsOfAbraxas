using Abraxas.Cards.Controllers;
using Abraxas.Cards.Models;
using Abraxas.Cells.Views;
using Abraxas.Events;
using Abraxas.Stones;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using Zenject;
using Image = UnityEngine.UI.Image;
using Player = Abraxas.Players.Players;

namespace Abraxas.Cards.Views
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(RectTransformMover))]
    [RequireComponent(typeof(CardDragListener))]
    [RequireComponent(typeof(CardMouseOverView))]
    class CardView : NetworkBehaviour, ICardView
    {
        #region Dependencies
        Card.Settings _cardSettings;
        Stone.Settings _stoneSettings;
        Players.Player.Settings _playerSettings;
        ICardModelReader _model;
        ICardController _controller;
        [Inject]
        public void Construct(Card.Settings cardSettings, Stone.Settings stoneSettings, Players.Player.Settings playerSettings)
        {
            _cardSettings = cardSettings;
            _stoneSettings = stoneSettings;
            _playerSettings = playerSettings;
        }
        public void Initialize(ICardModelReader model, ICardController controller)
        {
            _model = model;
            _controller = controller;

            _model.OnTitleChanged += OnTitleChanged;
            _model.OnOwnerChanged += OnOwnerChanged;
            _model.OnOriginalOwnerChanged += OnOriginalOwnerChanged;
            _model.OnHiddenChanged += OnHiddenChanged;
            OnTitleChanged();
            OnOwnerChanged();
            OnOriginalOwnerChanged();
            OnHiddenChanged();

            _image.sprite = _cardSettings.images[_model.ImageIndex];
        }
        public override void OnDestroy()
        {
            _model.OnTitleChanged -= OnTitleChanged;
            _model.OnOwnerChanged -= OnOwnerChanged;
            _model.OnOriginalOwnerChanged -= OnOriginalOwnerChanged;
            _model.OnHiddenChanged -= OnHiddenChanged;
        }
        #endregion

        #region Fields
        [SerializeField]
        Image _cover, _cardBack;
        [SerializeField]
        TMP_Text _titleText, _costText;
        [SerializeField]
        Image _image;
        RectTransformMover _rectTransformMover;
        #endregion

        #region Properties
        public ICardController Controller { get => _controller; }
        public RectTransformMover RectTransformMover { get => _rectTransformMover != null ? _rectTransformMover :_rectTransformMover = GetComponent<RectTransformMover>(); }
        public Image Image { get => _image; set => _image = value; }
        public Transform Transform => transform;
        #endregion

        #region Methods
        public void OnTitleChanged()
        {
            _titleText.text = _controller.Title;
        }
        public void OnOwnerChanged()
        {
            UnityEngine.Color playerColor = _playerSettings.GetPlayerDetails(_controller.Owner).color;
            _titleText.color = playerColor;
            _cover.color = playerColor;
            _cardBack.color = playerColor;
        }
        public void OnOriginalOwnerChanged()
        {
            Image.transform.localScale = new Vector3(_controller.OriginalOwner == Player.Player1 ? 1 : -1, 1, 1);
        }
        public void OnHiddenChanged()
        {
            _cover.gameObject.SetActive(_controller.Hidden);
        }
        public void UpdateCostTextWithCastability(ManaModifiedEvent eventData)
        {
            string TotalCost = "";
            foreach (var (pair, alpha) in from KeyValuePair<StoneType, int> pair in _model.TotalCosts
                                          from Manas.ManaType manaPair in eventData.Mana.ManaTypes
                                          where pair.Key == manaPair.Type
                                          let alpha = pair.Value <= manaPair.Amount ? "FF" : "44"
                                          select (pair, alpha))
            {
                TotalCost += $"<#{ColorUtility.ToHtmlStringRGB(_stoneSettings.GetStoneDetails(pair.Key).color)}{alpha}>{pair.Value}";
            }
            _costText.text = TotalCost;
        }
        public string GetCostText()
        {
            var costStrings = _controller.TotalCosts
                .Where(pair => pair.Value != 0)
                .Select(pair => $"<#{ColorUtility.ToHtmlStringRGB(_stoneSettings.GetStoneDetails(pair.Key).color)}>{pair.Value}");

            return string.Join("", costStrings);
        }
        
        public void ChangeScale(PointF scale, float time)
        {
            StartCoroutine(RectTransformMover.ChangeScaleEnumerator(new Vector2(scale.X, scale.Y), time));
        }
        public void SetCardPositionToMousePosition()
        {
            RectTransformMover.SetPosition(Input.mousePosition);
        }
        public IEnumerator MoveToCell(ICellView cell, float moveCardTime)
        {
            yield return RectTransformMover.MoveToFitRectangle(cell.RectTransform, moveCardTime);
        }
        #endregion
    }
}
