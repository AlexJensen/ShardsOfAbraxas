using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

using Abraxas.Stones;

using Player = Abraxas.Players.Players;
using Abraxas.Events;
using Abraxas.Cards.Models;
using Abraxas.Cards.Controllers;
using Unity.Netcode;

namespace Abraxas.Cards.Views
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(RectTransformMover))]
    [RequireComponent(typeof(CardDragListener))]
    [RequireComponent(typeof(CardMouseOverListener))]
    class CardView : NetworkBehaviour, ICardView
    {
        #region Dependencies
        Stone.Settings _stoneSettings;
        Players.Player.Settings _playerSettings;
        ICardModelReader _model;
        ICardController _controller;
        [Inject]
        public void Construct(ICardModelReader model, ICardController controller, Stone.Settings stoneSettings, Players.Player.Settings playerSettings)
        {
            _model = model;
            _controller = controller;
            _stoneSettings = stoneSettings;
            _playerSettings = playerSettings;

            Model.OnTitleChanged += OnTitleChanged;
            Model.OnOwnerChanged += OnOwnerChanged;
            Model.OnOriginalOwnerChanged += OnOriginalOwnerChanged;
            Model.OnHiddenChanged += OnHiddenChanged;
        }

        public override void OnDestroy()
        {
            Model.OnTitleChanged -= OnTitleChanged;
            Model.OnOwnerChanged -= OnOwnerChanged;
            Model.OnOriginalOwnerChanged -= OnOriginalOwnerChanged;
            Model.OnHiddenChanged -= OnHiddenChanged;
        }
        #endregion

        #region Fields
        [SerializeField]
        GameObject _cover;
        [SerializeField]
        TMP_Text _titleText, _costText;
        [SerializeField]
        Image _image;

        RectTransformMover _rectTransformMover;
        #endregion

        #region Properties
        public ICardController Controller { get => _controller; }
        public ICardModelReader Model { get => _model; }
        public RectTransformMover RectTransformMover { get => _rectTransformMover != null ? _rectTransformMover :_rectTransformMover = GetComponent<RectTransformMover>(); }
        public Image Image { get => _image; set => _image = value; }

        public Transform Transform => transform;

        public NetworkBehaviourReference NetworkBehaviourReference => NetworkBehaviourReference;
        #endregion

        #region Methods
        public void OnTitleChanged()
        {
            _titleText.text = Model.Title;
        }

        public void OnOwnerChanged()
        {
            _titleText.color = _playerSettings.GetPlayerDetails(Model.Owner).color;
        }

        public void OnOriginalOwnerChanged()
        {
            Image.transform.localScale = new Vector3(Model.OriginalOwner == Player.Player1 ? 1 : -1, 1, 1);
        }

        public void OnHiddenChanged()
        {
            _cover.SetActive(Model.Hidden);
        }

        public void UpdateVisuals()
        {
            _titleText.text = Model.Title;
            Image.transform.localScale = new Vector3(Model.OriginalOwner == Player.Player1 ? 1 : -1, 1, 1);
            _titleText.color = _playerSettings.GetPlayerDetails(Model.Owner).color;
            _costText.text = GetCostText();
            _cover.SetActive(Model.Hidden);
        }

        public void UpdateCostTextWithCastability(ManaModifiedEvent eventData)
        {
            string TotalCost = "";
            foreach (var (pair, alpha) in from KeyValuePair<StoneType, int> pair in Model.TotalCosts
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
            var costStrings = Model.TotalCosts
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
            RectTransformMover.SetCardPosition(Input.mousePosition);
        }
        #endregion
    }
}
