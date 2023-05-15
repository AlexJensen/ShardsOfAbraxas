using Abraxas.Stones;
using System.Collections.Generic;

using Player = Abraxas.Players.Players;
using System.Drawing;
using System;
using Abraxas.Cells.Controllers;
using Abraxas.Zones.Controllers;

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
        public IZoneController Zone { get; }
        public bool Hidden { get; }
        Point FieldPosition { get; }
        ICellController Cell { get; }

        event Action OnTitleChanged;
        event Action OnOwnerChanged;
        event Action OnOriginalOwnerChanged;
        event Action OnHiddenChanged;
    }
}
