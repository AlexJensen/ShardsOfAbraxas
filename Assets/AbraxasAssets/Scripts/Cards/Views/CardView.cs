using Abraxas.Cards.Models;
using Abraxas.Cells.Controllers;
using Abraxas.StatBlocks;
using Abraxas.Stones;
using Abraxas.Unity.Interfaces;
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
    [RequireComponent(typeof(CardMouseOverListener))]
    class CardView : NetworkBehaviour, ICardView, ITransformManipulator, IImageManipulator
    {
        #region Dependencies
        Stone.Settings _stoneSettings;
        Players.Player.Settings _playerSettings;
        Statblock.Settings _statblockSettings;

        ICardModel _model;


        [Inject]
        public void Construct(Stone.Settings stoneSettings, Players.Player.Settings playerSettings, Statblock.Settings statblockSettings)
        {
            _stoneSettings = stoneSettings;
            _playerSettings = playerSettings;
            _statblockSettings = statblockSettings;
        }

        public void Initialize(ICardModel model)
        {
            _model = model;

            _model.OnTitleChanged += OnTitleChanged;
            _model.OnOwnerChanged += OnOwnerChanged;
            _model.OnOriginalOwnerChanged += OnOriginalOwnerChanged;
            _model.OnHiddenChanged += OnHiddenChanged;
            OnTitleChanged();
            OnOwnerChanged();
            OnOriginalOwnerChanged();
            OnHiddenChanged();

            _image.sprite = _statblockSettings.GetSprite(_model.StatBlock.Stats);
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
        [SerializeField]
        GameObject _highlight;
        RectTransformMover _rectTransformMover;

        #endregion

        #region Properties
        public RectTransformMover RectTransformMover => _rectTransformMover = _rectTransformMover != null ? _rectTransformMover : GetComponent<RectTransformMover>();

        public Image Image { get => _image; set => _image = value; }
        public Transform Transform => transform;
        #endregion

        #region Methods
        public void OnTitleChanged()
        {
            _titleText.text = _model.Title;
        }

        public void OnOwnerChanged()
        {
            UnityEngine.Color playerColor = _playerSettings.GetPlayerDetails(_model.Owner).color;
            _titleText.color = playerColor;
            _cover.color = playerColor;
            _cardBack.color = playerColor;
        }

        public void OnOriginalOwnerChanged()
        {
            Image.transform.localScale = new Vector3(_model.OriginalOwner == Player.Player1 ? 1 : -1, 1, 1);
        }

        public void OnHiddenChanged()
        {
            _cover.gameObject.SetActive(_model.Hidden);
        }

        public void UpdateCostText(string totalCost)
        {
            _costText.text = totalCost;
        }

        public void SetHighlight(bool isPlayable)
        {
            _highlight.SetActive(isPlayable);
        }

        public void UpdateCostTextWithManaTypes(List<Manas.ManaType> manaTypes, Dictionary<StoneType, int> totalCosts, bool isPlayable)
        {
            string totalCost = "";
            foreach (var (pair, manaPair) in from pair in totalCosts
                                             let manaPair = manaTypes.FirstOrDefault(m => m.Type == pair.Key)
                                             select (pair, manaPair))
            {
                if (manaPair != null)
                {
                    string alpha = pair.Value <= manaPair.Amount ? "FF" : "44";
                    totalCost += $"<#{ColorUtility.ToHtmlStringRGB(_stoneSettings.GetStoneTypeDetails(pair.Key).color)}{alpha}>{pair.Value}";
                }
            }
            UpdateCostText(totalCost);
            SetHighlight(isPlayable);
        }

        public string GetCostText()
        {
            var costStrings = _model.TotalCosts
                .Where(pair => pair.Value != 0)
                .Select(pair => $"<#{ColorUtility.ToHtmlStringRGB(_stoneSettings.GetStoneTypeDetails(pair.Key).color)}>{pair.Value}");

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

        public IEnumerator MoveToCell(ICellController cell, float moveCardTime)
        {
            yield return RectTransformMover.MoveToFitRectangle(cell.RectTransform, moveCardTime);
        }

        public void SetToInitialScale()
        {
            RectTransformMover.SetToInitialScale();
        }
        #endregion
    }
}
