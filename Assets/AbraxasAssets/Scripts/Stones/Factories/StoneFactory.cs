using Abraxas.Stones.Controllers;
using Abraxas.Stones.Data;
using Abraxas.Stones.Models;
using System;
using Zenject;

namespace Abraxas.Stones.Factories
{
    class StoneFactory : IFactory<StoneDataSO, IStoneController>
	{
		private readonly DiContainer _container;
		private readonly Stone.Settings _settings;

		public StoneFactory(DiContainer container, Stone.Settings settings)
		{
			_container = container;
			_settings = settings;
		}

		public IStoneController Create(StoneDataSO dataSO)
		{
			StoneDataSO matchingDataSO = _settings.stoneData.Find(so => so.name == dataSO.name);

			if (matchingDataSO != null)
			{
				var controllerType = matchingDataSO.ControllerType;
				if (controllerType != null && typeof(IStoneController).IsAssignableFrom(controllerType))
				{
					var controller = (IStoneController)_container.Instantiate(controllerType);
					var model = _container.Instantiate<StoneModel>();
					model.Initialize(matchingDataSO.Data);
					controller.Initialize(model);
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