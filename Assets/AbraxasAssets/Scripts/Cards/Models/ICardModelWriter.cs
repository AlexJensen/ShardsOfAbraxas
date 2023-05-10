using Abraxas.Stones;
using Abraxas.Zones.Fields;
using System.Collections.Generic;
using System.Drawing;

using Zone = Abraxas.Zones.Zones;
using Player = Abraxas.Players.Players;


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
        public Zone Zone { set; }
        public bool Hidden { set; }
        ICellController Cell { set; }
        #endregion
    }
}
