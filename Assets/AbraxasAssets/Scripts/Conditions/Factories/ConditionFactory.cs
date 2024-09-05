using Abraxas.Stones.Conditions;
using Abraxas.Stones.Controllers;
using Abraxas.Stones.Targets;
using System;
using System.Reflection;
using Zenject;

namespace Abraxas.Conditions.Factories
{
    class ConditionFactory : IFactory<ConditionSOBase, IStoneController, ICondition>
    {
        private readonly DiContainer _container;
        private readonly TargetSOBase.Factory _targetFactory;

        public ConditionFactory(DiContainer container, TargetSOBase.Factory targetFactory)
        {
            _container = container;
            _targetFactory = targetFactory;
        }

        public ICondition Create(ConditionSOBase conditionSO, IStoneController stone)
        {
            if (conditionSO == null)
            {
                throw new ArgumentNullException(nameof(conditionSO), "ConditionSO cannot be null.");
            }

            // Use the container to instantiate the specific condition type
            if (_container.Instantiate(conditionSO.GetType()) is not ICondition conditionInstance)
            {
                throw new InvalidOperationException($"Could not instantiate condition of type '{conditionSO.GetType()}'");
            }

            // Initialize the condition
            if (conditionInstance is ConditionSOBase conditionSOBase)
            {
                conditionSOBase.Initialize(stone, conditionSOBase, _container);
            }
            else
            {
                throw new InvalidOperationException($"Condition of type '{conditionSO.GetType()}' does not inherit from ConditionSOBase");
            }

            // Initialize any nested conditions or targets
            InitializeNestedElements(conditionInstance, stone);

            return conditionInstance;
        }

        private void InitializeNestedElements(ICondition condition, IStoneController stone)
        {
            var fields = condition.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (typeof(ConditionSOBase).IsAssignableFrom(field.FieldType))
                {
                    var nestedCondition = field.GetValue(condition) as ConditionSOBase;
                    if (nestedCondition != null)
                    {
                        var nestedConditionInstance = Create(nestedCondition, stone);
                        field.SetValue(condition, nestedConditionInstance);
                    }
                }
                else if (typeof(TargetSOBase).IsAssignableFrom(field.FieldType))
                {
                    var targetInstance = field.GetValue(condition) as TargetSOBase;
                    if (targetInstance != null)
                    {
                        _targetFactory.Create(targetInstance as TargetSO<object>, stone);
                    }
                }
            }
        }
    }
}
