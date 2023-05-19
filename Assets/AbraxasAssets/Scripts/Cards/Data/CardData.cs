using Abraxas.StatBlocks.Data;
using Abraxas.Stones.Data;
using System;
using System.Collections.Generic;

namespace Abraxas.Cards.Data
{
    [Serializable]
    public struct CardData
    {
        public string Title { get; set; }
        public List<StoneData> Stones { get; set; }
        public StatBlockData StatBlock { get; set; }
    }
}
