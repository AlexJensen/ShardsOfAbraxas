using Abraxas.Stones.Controllers;
using System;
using UnityEngine;
using Zenject;

namespace Abraxas.Stones.Targets
{
    abstract class TargetSOBase : ScriptableObject, ITarget
    {
        [SerializeField]
        private string _expectedTypeName;

        public virtual Type ResolveRuntimeType()
        {
            // default fallback to the T in TargetSO<T>, e.g. “object” or “ICardController”
            var baseType = GetType().BaseType;
            if (baseType != null && baseType.IsGenericType)
            {
                return baseType.GetGenericArguments()[0];
            }
            return typeof(object);
        }

        public Type ExpectedType
        {
            get
            {
                if (string.IsNullOrEmpty(_expectedTypeName))
                    return null;
                return Type.GetType(_expectedTypeName);
            }
        }

        public void SetExpectedType(Type t)
        {
            if (t == null)
            {
                _expectedTypeName = string.Empty;
            }
            else
            {
                _expectedTypeName = t.AssemblyQualifiedName;
            }
        }

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