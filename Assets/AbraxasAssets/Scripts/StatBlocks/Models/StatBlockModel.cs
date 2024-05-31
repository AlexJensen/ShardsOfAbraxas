using Abraxas.StatBlocks.Data;
using Abraxas.Stones;
using NSubstitute.Routing.Handlers;
using System;
using UnityEngine;

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

        #region Properties
        public string StatsStr => _data.Stats.ATK.ToString() + "/" + _data.Stats.DEF.ToString() + "/" + _data.Stats.SPD.ToString()+ "/" + _data.Stats.RNG.ToString();

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
                OnStatsChanged.Invoke();
            }
        }

        public StatData Stats { get => _data.Stats; set => _data.Stats = value; }
        #endregion
    }
}
