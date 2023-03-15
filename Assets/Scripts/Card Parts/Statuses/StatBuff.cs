using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

public class StatBuff : Status
{
    StatBlock statBlock;
    Vector3Int buff;

    public StatBuff(StatBlock statBlock, Vector3Int buff)
    {
        Set(statBlock, buff);
    }

    ~StatBuff()
    {
        Clear(statBlock, buff);
    }

    public override void Set(params object[] vals)
    {
        if (vals[0] is not StatBlock)
        {
            Debug.LogWarning(GetType() + ": " + System.Reflection.MethodBase.GetCurrentMethod().Name +  " Exception! " + vals[0].GetType() + " was not a Card!");
            return;
        }
        if (vals[1] is not Vector3Int)
        {
            Debug.LogWarning(GetType() + ": " + System.Reflection.MethodBase.GetCurrentMethod().Name + " Exception! " + vals[1].GetType() + " was not a Vector3Int!");
            return;
        }
        statBlock = (StatBlock)vals[0];
        buff = (Vector3Int)vals[1];
        statBlock.Stats += buff;
    }
    public override void Clear(params object[] vals)
    {
        if (vals[0] is not StatBlock)
        {
            Debug.LogWarning(GetType() + ": " + System.Reflection.MethodBase.GetCurrentMethod().Name + " Exception! " + vals[0].GetType() + " was not a Card!");
            return;
        }
        if (vals[1] is not Vector3Int)
        {
            Debug.LogWarning(GetType() + ": " + System.Reflection.MethodBase.GetCurrentMethod().Name + " Exception! " + vals[1].GetType() + " was not a Vector3Int!");
            return;
        }
        statBlock = (StatBlock)vals[0];
        buff = (Vector3Int)vals[1];
        statBlock.Stats -= buff;
    }
}
