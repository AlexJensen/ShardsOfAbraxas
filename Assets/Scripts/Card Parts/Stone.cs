using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Stone : MonoBehaviour
{
    protected int cost;
    protected string info;
    protected StoneData.StoneType stoneType;

    [HideInInspector]
    public Card card;

    public int Cost { get => cost; }
    public string Info { get => info; }
    public StoneData.StoneType StoneType { get => stoneType; }
}
