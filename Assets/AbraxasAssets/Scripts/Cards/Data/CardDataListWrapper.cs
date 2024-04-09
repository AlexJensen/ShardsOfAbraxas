using System;
using System.Collections.Generic;
using Player = Abraxas.Players.Players;

namespace Abraxas.Cards.Data
{
	[Serializable]
    public class CardDataListWrapper
    {
		public Player Player;
		public List<CardData> CardDataList;
    }
}
