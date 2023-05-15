using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Abraxas
{
    public interface IHealthManager
    {
        void ModifyPlayerHealth(Players.Players player, int amount);
    }
}
