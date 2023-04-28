using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

using Abraxas.Core;
using Abraxas.Game;
using Abraxas.Stones;
using Abraxas.Zones.Fields;

using Zone = Abraxas.Zones.Zones;
using Player = Abraxas.Players.Players;

using Abraxas.Players;

namespace Abraxas.Cards
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(StatBlock))]
    [RequireComponent(typeof(RectTransformMover))]
    [RequireComponent(typeof(CardDragHandler))]
    [RequireComponent(typeof(CardMouseOverHandler))]
    public class Card : NetworkBehaviour, ICard
    {
        #region Settings
        Settings _settings;
        [Serializable]
        public class Settings
        {
            public float MovementOnFieldTime;
            public float ScaleToFieldCellTime;
        }
        #endregion

        #region Dependencies
        Stone.Settings _stoneSettings;
        Players.Player.Settings _playerSettings;
        IGameManager _gameManager;
        IPlayerManager _playerManager;
        IFieldManager _fieldManager;

        [Inject]
        public void Construct(Settings settings, Stone.Settings stoneSettings, Players.Player.Settings playerSettings, IGameManager gameManager, IPlayerManager playerManager, IFieldManager fieldManager)
        {
            _settings = settings;
            _stoneSettings = stoneSettings;
            _playerSettings = playerSettings;
            _gameManager = gameManager;
            _playerManager = playerManager;
            _fieldManager = fieldManager;
        }
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
        Dictionary<StoneType, int> _totalCosts;
        string _totalCostText;
        Zone _zone;
        List<Stone> _stones;
        StatBlock _statBlock;
        RectTransformMover _rectTransformMover;
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
                Color controllerColor = _playerSettings.GetPlayerDetails(Controller).color;
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
        public List<Stone> Stones => _stones ??= GetComponents<Stone>().ToList();
        public StatBlock StatBlock => _statBlock = _statBlock != null ? _statBlock : GetComponent<StatBlock>();
        public Image Image => _image;
        
        
        
        public Dictionary<StoneType, int> TotalCosts
        {
            get => _totalCosts ??= GenerateTotalCost(); 
            private set => _totalCosts = value; 
        }

        public Cell Cell { get; set; }
        public Vector2Int FieldPosition { get => _fieldPosition; set => _fieldPosition = value; }
        public Zone Zone { get => _zone; set => _zone = value; }
        public bool Hidden { get => _hidden; set => _hidden = value; }
        public RectTransformMover RectTransformMover => _rectTransformMover != null ? _rectTransformMover : _rectTransformMover = GetComponent<RectTransformMover>();
        #endregion

        #region Methods
        void Awake()
        {
            Controller = _controller;
            Owner = _owner;
        }

        private Dictionary<StoneType, int> GenerateTotalCost()
        {
            _totalCosts = new Dictionary<StoneType, int>();
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
            foreach (KeyValuePair<StoneType, int> pair in TotalCosts)
            {
                if (pair.Value != 0)
                {
                    TotalCost += "<#" + ColorUtility.ToHtmlStringRGB(_stoneSettings.GetStoneDetails(pair.Key).color) + ">" + pair.Value;
                }
            }
            TotalCostText = TotalCost;
        }

        public IEnumerator PassHomeRow()
        {
            _playerManager.ModifyPlayerHealth(Controller ==
                Player.Player1 ? Player.Player2 : Player.Player1,
                -StatBlock[StatBlock.StatValues.ATK]);
            yield return _gameManager.MoveCardFromFieldToDeck(this);
        }

        public IEnumerator Fight(Card opponent)
        {
            StatBlock collidedStats = opponent.GetComponent<StatBlock>();
            collidedStats[StatBlock.StatValues.DEF] -= StatBlock[StatBlock.StatValues.ATK];
            StatBlock[StatBlock.StatValues.DEF] -= collidedStats[StatBlock.StatValues.ATK];


            yield return Utilities.WaitForCoroutines(this,
                opponent.CheckDeath(),
                CheckDeath());
        }

        private IEnumerator CheckDeath()
        {
            if (StatBlock[StatBlock.StatValues.DEF] <= 0)
            {
                yield return _gameManager.MoveCardFromFieldToGraveyard(this);
            }
        }

        public IEnumerator Combat()
        {
            yield return _fieldManager.MoveCardAndFight(this, new Vector2Int(
                Controller == Player.Player1 ? StatBlock[StatBlock.StatValues.MV] :
                Controller == Player.Player2 ? -StatBlock[StatBlock.StatValues.MV] : 0, 0));
        }
        #endregion

        #region Server Methods
        [ServerRpc(RequireOwnership = false)]
        public void RequestMoveToCellServerRpc(Vector2Int cell)
        {
            if (!IsServer) return;
            MoveToCellClientRpc(cell);
        }

        [ClientRpc]
        void MoveToCellClientRpc(Vector2Int cell)
        {
            if (!IsClient) return;
            StartCoroutine(_gameManager.MoveCardFromHandToCell(this, cell));
        }
        #endregion
    }
}