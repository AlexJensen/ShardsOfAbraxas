using Abraxas.Popups.Managers;
using Zenject;

namespace Abraxas.Popups.Installers
{
    internal class PopupWindowInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PopupWindowManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}
