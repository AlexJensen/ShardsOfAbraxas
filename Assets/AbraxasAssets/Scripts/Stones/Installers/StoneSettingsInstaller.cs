using UnityEngine;
using Zenject;

namespace Abraxas.Stones.Installers
{
    [CreateAssetMenu(fileName = "StoneSettings", menuName = "Abraxas/Stone Settings")]
    class StoneSettingsInstaller : ScriptableObjectInstaller<StoneSettingsInstaller>
    {
        public Stone.Settings StoneSettings;
        public override void InstallBindings()
        {
            Container.BindInstance(StoneSettings).AsSingle();
        }
    }
}
