using Abraxas.Events;
using Abraxas.Stones.Conditions;
using Abraxas.Stones.Controllers;
using Abraxas.Stones.Targets;
using System;
using System.Reflection;
using Zenject;

namespace Abraxas.Conditions.Factories
{
    class ConditionFactory : IFactory<ConditionSO<IEvent>, IStoneController, ICondition>
    {
        private readonly DiContainer _container;
        private readonly TargetSOBase.Factory _targetFactory;

        public ConditionFactory(DiContainer container, TargetSOBase.Factory targetFactory)
        {
            _container = container;
            _targetFactory = targetFactory;
        }

        public ICondition Create(ConditionSO<IEvent> conditionSO, IStoneController stone)
        {
            if (conditionSO == null)
            {
                throw new ArgumentNullException(nameof(conditionSO), "ConditionSO cannot be null.");
            }

            // Instantiate the condition scriptable object
            var conditionInstance = _container.Instantiate(conditionSO.GetType()) as ConditionSO<IEvent>;

            if (conditionInstance == null)
            {
                throw new InvalidOperationException($"Could not instantiate condition of type '{conditionSO.GetType()}'");
            }

            // Initialize the condition and recursively initialize any nested conditions or targets
            InitializeCondition(conditionInstance, stone);

            return conditionInstance;
        }

        private void InitializeCondition(ConditionSO<IEvent> conditionInstance, IStoneController stone)
        {
            var container = _container;
            conditionInstance.Initialize(stone, conditionInstance, container);

            var fields = conditionInstance.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                if (typeof(ConditionSO<IEvent>).IsAssignableFrom(field.FieldType))
                {
                    var nestedCondition = field.GetValue(conditionInstance) as ConditionSO<IEvent>;
                    if (nestedCondition != null)
                    {
                        var nestedConditionInstance = Create(nestedCondition, stone);
                        field.SetValue(conditionInstance, nestedConditionInstance);
                    }
                }
                else if (typeof(TargetSOBase).IsAssignableFrom(field.FieldType))
                {
                    var targetInstance = field.GetValue(conditionInstance) as TargetSOBase;
                    if (targetInstance != null)
                    {
                        _targetFactory.Create(targetInstance as TargetSO<object>, stone);
                    }
                }
            }
        }
    }
}
