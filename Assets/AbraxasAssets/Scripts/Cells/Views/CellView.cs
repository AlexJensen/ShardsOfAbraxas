using Abraxas.Cells.Controllers;
using Abraxas.Events;
using Abraxas.Events.Managers;
using Abraxas.Players.Managers;
using Abraxas.Random.Managers;
using System.Collections;
using System.Drawing;
using TMPro;
using UnityEngine;
using Zenject;
using Image = UnityEngine.UI.Image;
using Player = Abraxas.Players.Players;
namespace Abraxas.Cells.Views
{
    class CellView : MonoBehaviour, ICellView,
        IGameEventListener<Event_LocalPlayerChanged>
    {
        #region Dependencies
        ICellController _controller;
        IRandomManager _randomManager;
        IPlayerManager _playerManager;
        IEventManager _eventManager;
        Cell.Settings _cellSettings;
        Players.Player.Settings _playerSettings;
        [Inject]
        public void Construct(IRandomManager randomManager, IPlayerManager playerManager, IEventManager eventManager, Cell.Settings cellSettings, Players.Player.Settings playerSettings)
        {
            _randomManager = randomManager;
            _playerManager = playerManager;
            _cellSettings = cellSettings;
            _playerSettings = playerSettings;
            _eventManager = eventManager;

            _eventManager.AddListener(typeof(Event_LocalPlayerChanged), this);
        }

        public void Initialize(ICellController controller)
        {
            _controller = controller;


            if (_cellBack != null)
            {
                _cellBack.sprite = _cellSettings.cellBackTextures[_randomManager.Range(0, _cellSettings.cellBackTextures.Length)];
            }
        }

        public void OnDestroy()
        {
            _eventManager.RemoveListener(typeof(Event_LocalPlayerChanged), this);
        }
        #endregion

        #region Fields
        [SerializeField]
        Player _player;
        [SerializeField]
        Image _cellBack;
        [SerializeField]
        TMP_Text _cellText;
        [SerializeField]
        RectTransform _cardHolder;
        #endregion

        #region Properties
        public ICellController Controller => _controller;
        public RectTransform RectTransform { get => _cardHolder; }

        public Player Player => _player;
        #endregion

        #region Methods
        public void FitRectTransformInCell(RectTransform rect)
        {
            rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, RectTransform.rect.width);
            rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, RectTransform.rect.height);
        }

        public void SetChild(Transform transform)
        {
            transform.SetParent(RectTransform);
            FitRectTransformInCell(RectTransform);
        }

        public void UpdateText(string Text)
        {
            _cellText.text = Text;
        }

        public IEnumerator OnEventRaised(Event_LocalPlayerChanged eventData)
        {
            UnityEngine.Color cellColor = _playerSettings.GetPlayerDetails(_playerManager.LocalPlayer == Player.Player1 ? _player : _player == Player.Neutral ? _player : (_player == Player.Player1 ? Player.Player2 : Player.Player1)).color;
            cellColor.a = 0.6f;
            _cellBack.color = cellColor;

            yield break;
        }

        public bool ShouldReceiveEvent(Event_LocalPlayerChanged eventData)
        {
            return true;
        }
        #endregion
    }
}