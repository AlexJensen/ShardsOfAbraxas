using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Abraxas.Manas.Views
{
    interface IManaView
    {
        Players.Players Player { get; }
        Transform Transform { get; }
    }
}
