using Abraxas.Random.Managers;
using Abraxas.Zones.Models;

namespace Abraxas.Zones.Graveyards.Models
{
	class GraveyardModel : ZoneModel, IGraveyardModel
	{
		public GraveyardModel(IRandomManager randomManager) : base(randomManager)
		{
		}
	}
}
