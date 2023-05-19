using Abraxas.StatBlocks.Data;
using Abraxas.Stones;
using System;

namespace Abraxas.StatBlocks.Models
{
    public interface IStatBlockModelReader
    {
        int this[StatValues index] { get; }

        string StatsStr { get; }
        StoneType StoneType { get; }
        int Cost { get; }

        event Action OnStatsChanged;

        public void Initialize(StatBlockData data);
    }
}
