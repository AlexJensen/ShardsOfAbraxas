using Abraxas.Cards.Controllers;
using Abraxas.Cards.Models;
using Abraxas.Cells.Controllers;
using Abraxas.Players.Managers;
using Abraxas.StatBlocks;
using Abraxas.Stones;
using Abraxas.UI;
using Abraxas.Unity.Interfaces;
using Abraxas.VFX;
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
    /// <summary>
    /// CardView contains all visual data and Unity-specific functionality for a card.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(RectTransformMover))]
    [RequireComponent(typeof(CardDragListener))]
    [RequireComponent(typeof(CardMouseOverListener))]
    [RequireComponent(typeof(AnimationClipPlayer))]
    class CardView : NetworkBehaviour, ICardView, ITransformManipulator, IImageManipulator
    {
        #region Dependencies
        Stone.Settings _stoneSettings;
        Players.Player.Settings _playerSettings;
        Statblock.Settings _statblockSettings;

        ICardModel _model;
        ICardController _controller;

        IPlayerManager _playerManager;
        [Inject]
        public void Construct(Stone.Settings stoneSettings, Players.Player.Settings playerSettings, Statblock.Settings statblockSettings, IPlayerManager playerManager)
        {
            _stoneSettings = stoneSettings;
            _playerSettings = playerSettings;
            _statblockSettings = statblockSettings;
            _playerManager = playerManager;
        }

        public void Initialize(ICardModel model, ICardController controller)
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

            _image.sprite = _statblockSettings.GetSprite(_model.StatBlock.Stats);
            _attack = _statblockSettings.GetAttackAnimation(_model.StatBlock.Stats);
            _image.material = _stoneSettings.GetStoneTypeDetails(_model.StatBlock.StoneType).material;
        }

        public override void OnDestroy()
        {
            _model.OnTitleChanged -= OnTitleChanged;
            _model.OnOwnerChanged -= OnOwnerChanged;
            _model.OnOriginalOwnerChanged -= OnOriginalOwnerChanged;
            _model.OnHiddenChanged -= OnHiddenChanged;
            _controller.OnDestroy();
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
        [SerializeField]
        AnimationClipPlayer _animationClipPlayer;
        AnimationClip _attack;
        RectTransformMover _rectTransformMover;

        #endregion

        #region Properties
        public RectTransformMover RectTransformMover => _rectTransformMover = _rectTransformMover != null ? _rectTransformMover : GetComponent<RectTransformMover>();
        public Image Image { get => _image; set => _image = value; }
        public Transform Transform => transform;

        public AnimationClip AttackAnimation { get => _attack; }
        #endregion

        #region Methods
        public void OnTitleChanged()
        {
            _titleText.text = _model.Title;
        }

        public void OnOwnerChanged()
        {
            UnityEngine.Color playerColor = _playerSettings.GetPlayerDetails(_playerManager.LocalPlayer == Player.Player1? _model.Owner : _model.Owner == Player.Player1? Player.Player2: Player.Player1).color;
            _titleText.color = playerColor;
            _cover.color = playerColor;
            _cardBack.color = playerColor;
        }

        public void OnOriginalOwnerChanged()
        {
            Image.transform.localScale = new Vector3((_playerManager.LocalPlayer == Player.Player1 ? _model.Owner : _model.Owner == Player.Player1 ? Player.Player2 : Player.Player1) == Player.Player1 ? 1 : -1, 1, 1);
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

        public IEnumerator PlayAnimation(AnimationClip clip, UnityEngine.Color color, bool flip)
        {
            yield return _animationClipPlayer.PlayAnimationAndWait(clip, color, flip);
        }

        public void UpdateCostTextWithManaTypes(List<Manas.ManaType> manaTypes, Dictionary<StoneType, int> totalCosts, bool isPlayable, bool isInHand)
        {
            string totalCost = "";
            foreach (var (pair, manaPair) in from pair in totalCosts
                                             let manaPair = manaTypes.FirstOrDefault(m => m.Type == pair.Key)
                                             select (pair, manaPair))
            {
                if (manaPair != null)
                {
                    string alpha = (pair.Value <= manaPair.Amount || !isInHand) ? "FF" : "44";
                    totalCost += $"<#{ColorUtility.ToHtmlStringRGB(_stoneSettings.GetStoneTypeDetails(pair.Key).color)}{alpha}>{pair.Value}";
                }
            }
            if (totalCost != "")
            {
                UpdateCostText(totalCost);
            }
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
