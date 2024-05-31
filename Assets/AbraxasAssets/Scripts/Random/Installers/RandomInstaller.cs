using Abraxas.Random.Managers;
using Zenject;

namespace Abraxas.Random.Installers
{

    /// <summary>
    /// RandomInstaller is a MonoInstaller that installs RandomManager into the Zenject container.
    /// </summary>
    internal class RandomInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<RandomManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}
