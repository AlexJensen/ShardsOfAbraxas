using Abraxas.StatBlocks.Data;
using Abraxas.Stones.Data;
using System;
using System.Collections.Generic;

using Player = Abraxas.Players.Players;

namespace Abraxas.Cards.Data
{
    [Serializable]
    public struct CardData
    {
        #region Fields
        public string Title;
        public Player Owner;
        public Player OriginalOwner;
        public List<StoneData> Stones;
        public StatBlockData StatBlock;
        #endregion
    }
}
