using Abraxas.Manas.Managers;
using Zenject;

namespace Abraxas.Manas.Installers
{
    class ManaInstaller : MonoInstaller
    {
        [Inject] private readonly Mana.Settings _manaSettings;
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ManaManager>().FromComponentInHierarchy().AsSingle();

            Container.BindFactory<ManaType, ManaType.Factory>().FromComponentInNewPrefab(_manaSettings.ManaTypePrefab);
        }
    }
}
