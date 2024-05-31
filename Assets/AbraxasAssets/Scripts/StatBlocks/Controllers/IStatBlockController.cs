using Abraxas.StatBlocks.Data;
using Abraxas.Stones;
using System.Numerics;
using UnityEngine;

namespace Abraxas.StatBlocks.Controllers
{

    /// <summary>
    /// IStatBlockController is an interface for controlling stat blocks.
    /// </summary>

    public interface IStatBlockController
    {
        string StatsStr { get; }
        StoneType StoneType { get; }
        int Cost { get; set; }

        StatData Stats { get; set; }

    }
}
