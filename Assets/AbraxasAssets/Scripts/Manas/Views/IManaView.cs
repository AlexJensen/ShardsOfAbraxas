
﻿using UnityEngine;

namespace Abraxas.Manas.Views
{
    /// <summary>
    /// IManaView is an interface for mana views.
    /// </summary>
    interface IManaView
    {
        Players.Players Player { get; }
        Transform Transform { get; }
    }
}
