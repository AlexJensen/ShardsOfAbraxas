using Abraxas.Cells.Controllers;
using Abraxas.Zones.Models;
using System.Collections.Generic;
using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Fields.Models
{
    class FieldModel: ZoneModel, IFieldModel
    {
        #region Fields
        List<List<ICellController>> _fieldGrid = new();
        #endregion

        #region Properties
        public List<List<ICellController>> FieldGrid { get => _fieldGrid; set => _fieldGrid = value; }

        public override void Shuffle()
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
