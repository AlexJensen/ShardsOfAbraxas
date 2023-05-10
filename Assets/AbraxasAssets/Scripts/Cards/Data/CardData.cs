using System.Collections.Generic;
using Abraxas.Stones;
using System;

namespace Abraxas.Cards.Data
{
    [Serializable]
    public struct CardData
    {
        public string Title { get; set; }
        public List<StoneData> Stones { get; set; }
        public StatBlock StatBlock { get; set; }
    }
}
