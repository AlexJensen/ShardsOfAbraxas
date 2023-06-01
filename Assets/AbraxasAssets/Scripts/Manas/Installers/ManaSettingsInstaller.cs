using UnityEngine;
using Zenject;

namespace Abraxas.Manas.Installers
{
    [CreateAssetMenu(fileName = "ManaSettings", menuName = "Abraxas/Mana Settings")]
    class ManaSettingsInstaller : ScriptableObjectInstaller<ManaSettingsInstaller>
    {
        public Mana.Settings ManaSettings;
        public override void InstallBindings()
        {
            Container.BindInstance(ManaSettings).AsSingle();
        }
    }
}
