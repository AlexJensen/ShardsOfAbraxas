using Abraxas.Behaviours.Stones;
using UnityEngine;


namespace Abraxas.Behaviours.Status
{
    public class StatBuff : Status
    {
        StatBlock _statBlock;
        Vector3Int _buff;

        public StatBuff(StatBlock statBlock, Vector3Int buff)
        {
            Set(statBlock, buff);
        }

        ~StatBuff()
        {
            Clear(_statBlock, _buff);
        }

        public override void Set(params object[] vals)
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
            _statBlock = (StatBlock)vals[0];
            _buff = (Vector3Int)vals[1];
            _statBlock.Stats += _buff;
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
            _statBlock = (StatBlock)vals[0];
            _buff = (Vector3Int)vals[1];
            _statBlock.Stats -= _buff;
        }
    }
}