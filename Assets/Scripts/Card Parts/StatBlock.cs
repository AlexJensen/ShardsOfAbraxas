using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StatBlock : MonoBehaviour
{
    public enum StatValues
    {
        ATK,
        DEF,
        MV
    }

    [SerializeField]
    Vector3Int stats;

    public StatBlock(Vector3Int stats) => this.Stats = stats;

    public string statsStr => this[StatValues.ATK].ToString() + "/" + this[StatValues.DEF].ToString() + "/" + this[StatValues.MV].ToString();

    public Vector3Int Stats { get => stats; set => stats = value; }

    public int this[StatValues index]
    {
        get => stats[(int)index];
        set => stats[(int)index] = value;
    }
}
