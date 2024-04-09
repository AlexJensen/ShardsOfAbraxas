using Abraxas.Cells.Controllers;
using Abraxas.StatBlocks.Controllers;
using Abraxas.Stones;
using Abraxas.Stones.Controllers;
using Abraxas.Zones.Controllers;
using System;
using System.Collections.Generic;
using System.Drawing;
using Player = Abraxas.Players.Players;

namespace Abraxas.Cards.Models
{
    public interface ICardModel
    {
        public string Title { get; set; }
        public Player Owner { get; set; }
        public Player OriginalOwner { get; set; }
        public List<IStoneController> Stones { get; set; }
        public IStatBlockController StatBlock { get; set; }
        public Dictionary<StoneType, int> TotalCosts { get; }
        public IZoneController Zone { get; set; }
        public bool Hidden { get; set; }
        Point FieldPosition { get; set; }
        ICellController Cell { get; set; }
        int ImageIndex { get; set; }

        event Action OnTitleChanged;
        event Action OnOwnerChanged;
        event Action OnOriginalOwnerChanged;
        event Action OnHiddenChanged;
    }
}
