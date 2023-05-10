using Abraxas.Stones;
using Abraxas.Zones.Fields;
using System.Collections.Generic;

using Zone = Abraxas.Zones.Zones;
using Player = Abraxas.Players.Players;
using System.Drawing;
using System;

namespace Abraxas.Cards.Models
{
    public interface ICardModelReader
    {
        public string Title { get; }
        public Player Owner { get; }
        public Player OriginalOwner { get;  }
        public List<IStoneController> Stones { get;  }
        public StatBlock StatBlock { get;  }
        public Dictionary<StoneType, int> TotalCosts { get;  }
        public Zone Zone { get; }
        public bool Hidden { get; }
        Point FieldPosition { get; }
        ICellController Cell { get; }

        event Action OnTitleChanged;
        event Action OnOwnerChanged;
        event Action OnOriginalOwnerChanged;
        event Action OnHiddenChanged;
    }
}
