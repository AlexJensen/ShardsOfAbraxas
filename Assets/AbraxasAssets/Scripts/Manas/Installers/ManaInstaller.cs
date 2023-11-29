using Abraxas.Manas.Controllers;
using Abraxas.Manas.Factories;
using Abraxas.Manas.Managers;
using Abraxas.Manas.Views;
using Zenject;

namespace Abraxas.Manas.Installers
{
    class ManaInstaller : MonoInstaller
    {
        [Inject] private readonly Mana.Settings _manaSettings;
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ManaManager>().FromComponentInHierarchy().AsSingle();

            Container.BindFactory<IManaView, IManaController, ManaController.Factory>().FromFactory<ManaFactory>();
            Container.BindFactory<ManaType, ManaType.Factory>().FromComponentInNewPrefab(_manaSettings.ManaTypePrefab);
        }
    }
}
