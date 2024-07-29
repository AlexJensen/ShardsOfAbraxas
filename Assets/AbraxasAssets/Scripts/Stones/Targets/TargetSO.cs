using Abraxas.Stones.Controllers;
using System;
using UnityEngine;

namespace Abraxas.Stones.Targets
{
    [Serializable]
    abstract class TargetSO<T> : ScriptableObject, ITarget
    {
        #region Properties
        public abstract T Target { get; set; }
        #endregion

        #region Methods
        public virtual void Initialize(StoneController stoneController)
        {
            //
        }
        #endregion
    }
}