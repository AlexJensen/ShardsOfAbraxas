using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Status
{
    public List<Card> cardsAffected;
    public abstract void Set(params object[] vals);
    public abstract void Clear(params object[] vals);
}
