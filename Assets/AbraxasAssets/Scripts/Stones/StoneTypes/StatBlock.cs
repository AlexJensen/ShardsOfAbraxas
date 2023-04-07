using Abraxas.Behaviours.Data;
using TMPro;
using UnityEngine;

namespace Abraxas.Behaviours.Stones
{
    public class StatBlock : Stone
    {
        public enum StatValues
        {
            ATK,
            DEF,
            MV
        }

        [SerializeField]
        int cost;
        [SerializeField]
        StoneData.StoneType stoneType;
        [SerializeField]
        Vector3Int stats;

        [SerializeField]
        TMP_Text statsText;

        public Vector3Int Stats { get => stats; set => stats = value; }
        public string StatsStr => this[StatValues.ATK].ToString() + "/" + this[StatValues.DEF].ToString() + "/" + this[StatValues.MV].ToString();

        public override int Cost { get => cost; set => cost = value; }
        public override StoneData.StoneType StoneType { get => stoneType; set => stoneType = value; }

        public StatBlock(Vector3Int stats) => Stats = stats;

        public void Awake()
        {
            RefreshVisuals();
        }

        public int this[StatValues index]
        {
            get => stats[(int)index];
            set
            {
                stats[(int)index] = value;
                RefreshVisuals();
            }
        }

        private void RefreshVisuals()
        {
            statsText.text = StatsStr;
            statsText.color = DataManager.Instance.GetStoneDetails(StoneType).color;
        }
    }
}
