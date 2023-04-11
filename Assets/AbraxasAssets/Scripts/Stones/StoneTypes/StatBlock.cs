using Abraxas.Behaviours.Data;
using TMPro;
using UnityEngine;
using Zenject;

namespace Abraxas.Behaviours.Stones
{
    public class StatBlock : Stone
    {
        #region Enums
        public enum StatValues
        {
            ATK,
            DEF,
            MV
        }
        #endregion

        #region Dependency Injections
        [Inject] readonly DataManager _dataManager;
        #endregion

        [SerializeField]
        int _cost;
        [SerializeField]
        StoneData.StoneType _stoneType;
        [SerializeField]
        Vector3Int _stats;
        [SerializeField]
        TMP_Text _statsText;

        public Vector3Int Stats { get => _stats; set => _stats = value; }
        public string StatsStr => this[StatValues.ATK].ToString() + "/" + this[StatValues.DEF].ToString() + "/" + this[StatValues.MV].ToString();

        public override int Cost { get => _cost; set => _cost = value; }
        public override StoneData.StoneType StoneType { get => _stoneType; set => _stoneType = value; }
        public override string Info { get => null; set => throw new System.NotImplementedException(); }

        public void Awake()
        {
            RefreshVisuals();
        }

        public int this[StatValues index]
        {
            get => _stats[(int)index];
            set
            {
                _stats[(int)index] = value;
                RefreshVisuals();
            }
        }

        private void RefreshVisuals()
        {
            _statsText.text = StatsStr;
            _statsText.color = _dataManager.GetStoneDetails(StoneType).color;
        }
    }
}
