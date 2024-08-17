using Abraxas.Events;
using Abraxas.Stones.Conditions;
using Abraxas.Stones.Controllers;
using Abraxas.Stones.Data;
using Abraxas.Stones.Models;
using Abraxas.Stones.Targets;
using System;
using System.Collections.Generic;
using System.Reflection;
using Zenject;

namespace Abraxas.Stones.Factories
{
    class StoneFactory : IFactory<StoneSO, IStoneController>
    {
        private readonly DiContainer _container;

        public StoneFactory(DiContainer container)
        {
            _container = container;
        }

        public IStoneController Create(StoneSO dataSO)
        {
            if (dataSO == null)
            {
                throw new ArgumentNullException(nameof(dataSO), "StoneSO cannot be null.");
            }

            var controllerType = dataSO.ControllerType;
            if (controllerType == null || !typeof(IStoneController).IsAssignableFrom(controllerType))
            {
                throw new InvalidOperationException($"Controller type '{controllerType}' is not assignable to IStoneController.");
            }

            var controller = (IStoneController)_container.Instantiate(controllerType);
            var model = _container.Instantiate<StoneModel>();
            model.Initialize(dataSO);
            controller.Initialize(model);

            // Set the target if the controller implements ITargetable
            SetTargetIfTargetable(controller, dataSO);

            // Initialize conditions if the controller has any
            InitializeConditionsIfPresent(controller, dataSO);

            return controller;
        }

        private void SetTargetIfTargetable(IStoneController controller, StoneSO dataSO)
        {
            if (controller is ITargetable<object> targetableController)
            {
                var targetField = FindFieldByType(dataSO.GetType(), typeof(TargetSO<>));
                if (targetField != null)
                {
                    targetableController.Target = (TargetSO<object>)targetField.GetValue(dataSO);
                }
            }
        }

        // ...

        private void InitializeConditionsIfPresent(IStoneController controller, StoneSO dataSO)
        {
            if (controller is IConditional conditionableController)
            {
                var conditionField = FindFieldByType(dataSO.GetType(), typeof(ConditionSO<>));
                if (conditionField != null)
                {
                    var conditionSO = (ConditionSO<IEvent>)conditionField.GetValue(dataSO);
                    conditionableController.Conditions = new List<ICondition> { conditionSO };
                    conditionSO.Initialize(controller, conditionSO );
                }
            }
        }

        private FieldInfo FindFieldByType(Type type, Type fieldType)
        {
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == fieldType)
                {
                    return field;
                }
            }
            return null;
        }
    }
}
