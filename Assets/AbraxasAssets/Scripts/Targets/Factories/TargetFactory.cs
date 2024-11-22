using Abraxas.Stones.Conditions;
using Abraxas.Stones.Controllers;
using Abraxas.Stones.Targets;
using System.Reflection;
using System;
using Zenject;
using Abraxas.Events;

namespace Abraxas.Targets.Factories
{
    internal class TargetFactory : IFactory<TargetSOBase, IStoneController, ITarget>
    {
        private readonly DiContainer _container;
        private readonly ConditionSO.Factory _conditionFactory;

        public TargetFactory(DiContainer container, ConditionSO.Factory conditionFactory)
        {
            _container = container;
            _conditionFactory = conditionFactory;
        }

        public ITarget Create(TargetSOBase targetSO, IStoneController stone)
        {
            if (targetSO == null)
            {
                throw new ArgumentNullException(nameof(targetSO), "TargetSO cannot be null.");
            }

            // Instantiate the target scriptable object
            var targetInstance = _container.Instantiate(targetSO.GetType()) as TargetSOBase ?? throw new InvalidOperationException($"Could not instantiate condition of type '{targetSO.GetType()}'");

            // Initialize the targets and recursively initialize any nested conditions or targets
            InitializeTarget(targetInstance, stone);

            return targetInstance;
        }

        private void InitializeTarget(TargetSOBase targetInstance, IStoneController stone)
        {

            targetInstance.Initialize(stone);

            var fields = targetInstance.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                if (typeof(ConditionSO).IsAssignableFrom(field.FieldType))
                {
                    var nestedCondition = field.GetValue(targetInstance) as ConditionSO;
                    if (nestedCondition != null)
                    {
                        var nestedConditionInstance = _conditionFactory.Create(nestedCondition, stone);
                        field.SetValue(targetInstance, nestedConditionInstance);
                    }
                }
                else if (typeof(TargetSOBase).IsAssignableFrom(field.FieldType))
                {
                    var nestedTarget = field.GetValue(targetInstance) as TargetSOBase;
                    if (nestedTarget != null)
                    {
                        Create(nestedTarget, stone);
                    }
                }
            }
        }
    }
}
