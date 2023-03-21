using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StatBlock : Stone
{
    [SerializeField]
    protected new int cost;
    [SerializeField]
    protected new StoneData.StoneType stoneType;
    public enum StatValues
    {
        ATK,
        DEF,
        MV
    }

    [SerializeField]
    Vector3Int stats;

    public StatBlock(Vector3Int stats) => this.Stats = stats;

    private void Awake()
    {
        base.cost = cost;
        base.stoneType = stoneType;
    }

    public string statsStr => this[StatValues.ATK].ToString() + "/" + this[StatValues.DEF].ToString() + "/" + this[StatValues.MV].ToString();

    public Vector3Int Stats { get => stats; set => stats = value; }

    public int this[StatValues index]
    {
        get => stats[(int)index];
        set => stats[(int)index] = value;
    }
}
