using Abraxas.Random.Managers;
using Abraxas.Zones.Models;

namespace Abraxas.Zones.Hands.Models
{
	class HandModel : ZoneModel, IHandModel
	{
		public HandModel(IRandomManager randomManager) : base(randomManager)
		{
		}
	}
}
