using Abraxas.Cells.Controllers;
using Abraxas.Zones.Models;
using System.Collections.Generic;

namespace Abraxas.Zones.Fields.Models
{
    interface IFieldModel : IZoneModel
    {
        List<List<ICellController>> FieldGrid { get; }
    }
}
