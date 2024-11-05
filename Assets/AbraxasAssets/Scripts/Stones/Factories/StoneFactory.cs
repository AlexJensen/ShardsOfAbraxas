using Abraxas.Cards.Controllers;
using Abraxas.Stones.Conditions;
using Abraxas.Stones.Controllers;
using Abraxas.Stones.Data;
using Abraxas.Stones.Models;
using Abraxas.Stones.Targets;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace Abraxas.Stones.Factories
{
    class StoneFactory : IFactory<StoneSO, ICardController, IStoneController>
    {
        private readonly DiContainer _container;
        private readonly ConditionSOBase.Factory _conditionFactory;

        public StoneFactory(DiContainer container, ConditionSOBase.Factory conditionFactory)
        {
            _container = container;
            _conditionFactory = conditionFactory;
        }

        public IStoneController Create(StoneSO dataSO, ICardController card)
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
            controller.Initialize(model, card);

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
                    ((TargetSOBase)targetField.GetValue(dataSO))?.Initialize(controller);
                }
            }
        }

        private void InitializeConditionsIfPresent(IStoneController controller, StoneSO dataSO)
        {
            if (controller is IConditional conditionableController)
            {
                var conditionField = FindFieldByType(dataSO.GetType(), typeof(List<ConditionSOBase>));
                if (conditionField != null)
                {
                    var conditionList = (List<ConditionSOBase>)conditionField.GetValue(dataSO);
                    var conditions = new List<ICondition>();

                    foreach (var conditionSO in conditionList)
                    {
                        if (conditionSO != null)
                        {
                            var conditionInstance = _conditionFactory.Create(conditionSO, controller);
                            conditions.Add(conditionInstance);
                        }
                        else
                        {
                            Debug.LogError($"Condition of type {conditionSO?.GetType()} is not a valid ConditionSOBase type.");
                        }
                    }

                    conditionableController.Conditions = conditions;
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
