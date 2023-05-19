using Abraxas.Cells.Controllers;
using Abraxas.StatBlocks.Models;
using Abraxas.Stones.Models;
using Abraxas.Zones.Controllers;
using System.Collections.Generic;
using System.Drawing;
using Player = Abraxas.Players.Players;

namespace Abraxas.Cards.Models
{
    public interface ICardModelWriter
    {
        #region Properties
        public string Title { set; }
        public Player Owner { set; }
        public Player OriginalOwner { set; }
        public Point FieldPosition { set; }
        public IZoneController Zone { set; }
        public bool Hidden { set; }
        ICellController Cell { set; }
        #endregion
    }
}
