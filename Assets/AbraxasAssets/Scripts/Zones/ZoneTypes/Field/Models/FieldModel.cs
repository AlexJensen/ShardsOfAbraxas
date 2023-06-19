using Abraxas.Cells.Controllers;
using Abraxas.Zones.Fields.Views;
using Abraxas.Zones.Models;
using System.Collections.Generic;

namespace Abraxas.Zones.Fields.Models
{
    class FieldModel: ZoneModel, IFieldModel
    {
        #region Dependencies
        public override void Initialize<TView>(TView view)
        {
            _fieldGrid = ((IFieldView)view).GenerateField();
        }
        #endregion

        #region Fields
        List<List<ICellController>> _fieldGrid = new();
        #endregion

        #region Properties
        public List<List<ICellController>> FieldGrid { get => _fieldGrid; }
        #endregion
    }
}
