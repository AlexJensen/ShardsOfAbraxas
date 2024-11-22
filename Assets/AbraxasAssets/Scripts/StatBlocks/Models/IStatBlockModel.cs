using Abraxas.StatBlocks.Data;
using Abraxas.Stones;
using System;

using System.Numerics;
using UnityEngine;

namespace Abraxas.StatBlocks.Models
{
    /// <summary>
    /// IStatBlockModel is an interface for the StatBlockModel class.
    /// </summary>
    public interface IStatBlockModel
    {
        string StatsStr { get; }
        string StatsStrSymbol { get; }
        StoneType StoneType { get; set; }
        int Cost { get; set; }
        StatData Stats { get; set; }
        bool ShowSymbols { get; set; }

        event Action OnStatsChanged;

        void Initialize(StatBlockData data);
    }
}
