using Abraxas.Stones.Controllers;
using System;
using UnityEngine;

namespace Abraxas.Stones.Targets
{
    public abstract class TargetSOBase : ScriptableObject, ITarget
    {
        public abstract object GetTarget();
        public virtual void Initialize(StoneController stoneController)
        {
            //
        }
    }

    [Serializable]
    abstract class TargetSO<T> : TargetSOBase
    {
        #region Properties
        public abstract T Target { get; set; }
        #endregion

        #region Methods
        public override object GetTarget()
        {
            return Target;
        }
        #endregion
    }
}