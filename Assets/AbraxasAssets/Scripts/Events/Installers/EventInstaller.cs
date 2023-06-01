using Abraxas.Events.Managers;
using Zenject;

namespace Abraxas.Events.Installers
{
    class EventInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<EventManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}
