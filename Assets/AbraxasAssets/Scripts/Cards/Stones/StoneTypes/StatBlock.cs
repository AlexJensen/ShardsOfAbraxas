using TMPro;
using UnityEngine;

namespace Abraxas.Stones
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

        #region Fields
        [SerializeField]
        int _cost;
        [SerializeField]
        StoneType _stoneType;
        [SerializeField]
        Vector3Int _stats;
        [SerializeField]
        TMP_Text _statsText;
        #endregion

        #region Properties
        public Vector3Int Stats { get => _stats; set => _stats = value; }
        public string StatsStr => this[StatValues.ATK].ToString() + "/" + this[StatValues.DEF].ToString() + "/" + this[StatValues.MV].ToString();

        public override int Cost { get => _cost; set => _cost = value; }
        public override StoneType StoneType { get => _stoneType; set => _stoneType = value; }
        public override string Info { get => null; set => throw new System.NotImplementedException(); }
        public int this[StatValues index]
        {
            get => _stats[(int)index];
            set
            {
                _stats[(int)index] = value;
                RefreshVisuals();
            }
        }
        #endregion

        #region Methods
        public void Awake()
        {
            RefreshVisuals();
        }

        private void RefreshVisuals()
        {
            _statsText.text = StatsStr;
            _statsText.color = settings.GetStoneDetails(StoneType).color;
        }
        #endregion
    }
}
