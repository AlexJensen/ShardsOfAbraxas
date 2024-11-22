using Abraxas.Stones.Controllers;
using System;
using UnityEngine;
using Zenject;

namespace Abraxas.Stones.Targets
{
    abstract class TargetSOBase : ScriptableObject, ITarget
    {
        public abstract object GetTarget();
        public virtual void Initialize(IStoneController stoneController) { }

        public class Factory : PlaceholderFactory<TargetSOBase, IStoneController, ITarget> { }
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