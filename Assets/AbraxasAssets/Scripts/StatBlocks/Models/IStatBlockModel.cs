using Abraxas.StatBlocks.Data;
using Abraxas.Stones;
using System;

namespace Abraxas.StatBlocks.Models
{
    public interface IStatBlockModel
    {
        int this[StatValues index] { set; get; }

        string StatsStr { get; }
        StoneType StoneType { get; set; }
        int Cost { get; set; }

        event Action OnStatsChanged;

        void Initialize(StatBlockData data);
    }
}
