using Abraxas.StatBlocks.Controllers;
using Abraxas.StatBlocks.Models;
using Abraxas.Stones;
using UnityEngine;

namespace Abraxas.StatBlocks.Views
{
    /// <summary>
    /// IStatBlockView is an interface for stat block views.
    /// </summary>
    interface IStatBlockView
    {
        Color GetStoneColor(StoneType stoneType);
    }
}
