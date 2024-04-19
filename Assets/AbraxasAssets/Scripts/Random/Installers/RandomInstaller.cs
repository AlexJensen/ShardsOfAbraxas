using Abraxas.Random.Managers;
using Zenject;

namespace Abraxas.Random.Installers
{
	internal class RandomInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<RandomManager>().FromComponentInHierarchy().AsSingle();
		}
	}
}
