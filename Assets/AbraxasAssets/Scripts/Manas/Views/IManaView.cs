
using Abraxas.Manas.Controllers;
using Abraxas.Manas.Models;
using System.Collections;
using UnityEngine;

namespace Abraxas.Manas.Views
{
    /// <summary>
    /// IManaView is an interface for mana views.
    /// </summary>
    interface IManaView
    {
        Players.Players Player { get; }
        Transform Transform { get; }

        void Initialize(IManaModel model, IManaController controller);
        void StartManaEventCoroutine(IEnumerator eventInvoke);
    }
}
