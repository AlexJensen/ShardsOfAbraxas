using Abraxas.StatBlocks.Data;
using Abraxas.Stones;
using System;

namespace Abraxas.StatBlocks.Models
{
    /// <summary>
    /// StatBlockModel contains all data that makes up a stat block.
    /// </summary>
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

        #region Fields
        private bool _showSymbols;
        #endregion

        #region Properties
        public bool ShowSymbols
        {
            get => _showSymbols;
            set
            {
                _showSymbols = value;
                OnStatsChanged.Invoke();
            }
        }
        public string StatsStrSymbol => _data.Stats.ATK.ToString() + "<sprite=0 tint=1>/" + _data.Stats.DEF.ToString() + "<sprite=1 tint=1>/" + _data.Stats.SPD.ToString()+ "<sprite=2 tint=1>/" + _data.Stats.RNG.ToString() + "<sprite=3 tint=1>";
        public string StatsStr => _data.Stats.ATK.ToString() + "/" + _data.Stats.DEF.ToString() + "/" + _data.Stats.SPD.ToString() + "/" + _data.Stats.RNG.ToString();

        public StoneType StoneType
        {
            get => _data.StoneType;
            set => _data.StoneType = value;
        }
        public int Cost
        {
            get
            {
                return _data.Cost;
            }
            set
            {
                _data.Cost = value;
            }
        }

        public StatData Stats
        {
            get => _data.Stats; 
            
            set
            {
                _data.Stats = value;
                OnStatsChanged.Invoke();
            }
        }
        #endregion
    }
}
