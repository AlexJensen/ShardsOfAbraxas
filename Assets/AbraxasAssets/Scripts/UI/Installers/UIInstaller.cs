using Abraxas.UI.Managers;
using Zenject;

namespace Abraxas.UI.Installers
{
    class UIInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UIManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}
