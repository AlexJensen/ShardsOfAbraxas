using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : Singleton<Hands>
{
    [SerializeField]
    List<Hand> hands;

    public List<Hand> PlayerHands { get => hands; }
}
