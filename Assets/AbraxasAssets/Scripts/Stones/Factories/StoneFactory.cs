
﻿using Abraxas.Events.Managers;
using Abraxas.Stones.Controllers;
using Abraxas.Stones.Controllers.StoneTypes.Conditions;
using Abraxas.Stones.Data;
using Abraxas.Stones.Models;
using System;
using System.Collections.Generic;
using Zenject;

namespace Abraxas.Stones.Factories
{
    class StoneFactory : IFactory<StoneDataSO, IStoneController>
    {
        private readonly DiContainer _container;

        public StoneFactory(DiContainer container)
        {
            _container = container;
        }

        public IStoneController Create(StoneDataSO dataSO)
        {
            if (dataSO != null)
            {
                var controllerType = dataSO.ControllerType;
                if (controllerType != null && typeof(IStoneController).IsAssignableFrom(controllerType))
                {
                    var controller = (IStoneController)_container.Instantiate(controllerType);
                    var model = _container.Instantiate<StoneModel>();
                    model.Initialize(dataSO.Data);
                    controller.Initialize(model);

                    if (controller is TriggerStone triggerStone)
                    {
                        var triggerDataSO = dataSO as TriggerStoneDataSO;
                        if (triggerDataSO != null)
                        {
                            var conditions = new List<ICondition<object>>();
                            foreach (var condition in triggerDataSO.Conditions)
                            {
                                var instantiatedCondition = (ICondition<object>)_container.Instantiate(condition.GetType());
                                instantiatedCondition.Construct(_container.Resolve<IEventManager>());
                                conditions.Add(instantiatedCondition);
                            }
                            triggerStone.InitializeConditions(conditions);
                        }
                    }

                    return controller;
                }
                else
                {
                    throw new InvalidOperationException($"Controller type '{controllerType}' is not assignable to IStoneController.");
                }
            }
            else
            {
                throw new InvalidOperationException($"No matching StoneDataSO found for IStoneData type '{dataSO.GetType()}'.");
            }
        }
    }
}