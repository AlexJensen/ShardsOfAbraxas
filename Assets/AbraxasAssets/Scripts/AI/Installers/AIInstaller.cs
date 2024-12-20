using Abraxas.AI.Managers;
using Zenject;

namespace Abraxas.AI.Installers
{
    class AIInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AIManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}
