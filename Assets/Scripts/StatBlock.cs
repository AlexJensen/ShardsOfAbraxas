using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StatBlock : MonoBehaviour
{
    [Serializable]
    public struct Stats
    {
        public int HP, ATK, MV;
    }

    public Stats stats;
}
