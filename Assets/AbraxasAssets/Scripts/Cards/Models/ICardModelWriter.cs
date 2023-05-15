

using System.Collections.Generic;
using System.Drawing;

using Player = Abraxas.Players.Players;
using Abraxas.Stones;
using Abraxas.Cells.Controllers;
using Abraxas.Zones.Controllers;

namespace Abraxas.Cards.Models
{
    public interface ICardModelWriter
    {
        #region Properties
        public string Title { set; }
        public Player Owner { set; }
        public Player OriginalOwner { set; }
        public List<IStoneController> Stones { set; }
        public StatBlock StatBlock { set; }
        public Point FieldPosition { set; }
        public IZoneController Zone { set; }
        public bool Hidden { set; }
        ICellController Cell { set; }
        #endregion
    }
}
