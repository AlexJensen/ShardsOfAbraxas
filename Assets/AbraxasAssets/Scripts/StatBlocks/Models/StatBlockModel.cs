using Abraxas.StatBlocks.Data;
using Abraxas.Stones;
using System;

namespace Abraxas.StatBlocks.Models
{
    class StatBlockModel : IStatBlockModel
    {
        #region Events
        public event Action OnStatsChanged;
        #endregion

        #region Dependencies
        StatBlockData _data;
        public void Initialize(StatBlockData data)
        {
            _data = data;
        }
        #endregion

        #region Properties
        public string StatsStr => _data[StatValues.ATK].ToString() + "/" + _data[StatValues.DEF].ToString() + "/" + _data[StatValues.MV].ToString();

        public StoneType StoneType => _data.StoneType;
        public int Cost
        {
            get
            {
                return _data.Cost;
            }
            set
            {
                OnStatsChanged.Invoke();
                _data.Cost = value;
            }
        }

        public int this[StatValues index]
        {
            get => _data[index];
            set
            {
                OnStatsChanged.Invoke();
                _data[index] = value;
            }
        }
        #endregion
    }
}
