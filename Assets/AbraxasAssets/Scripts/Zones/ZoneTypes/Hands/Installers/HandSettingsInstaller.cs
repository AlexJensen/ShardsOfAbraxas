using UnityEngine;
using Zenject;

namespace Abraxas.Hands.Installers
{
    [CreateAssetMenu(fileName = "HandSettings", menuName = "Abraxas/Hand Settings")]
    class HandSettingsInstaller : ScriptableObjectInstaller<HandSettingsInstaller>
    {
        public Hand.Settings HandSettings;
        public override void InstallBindings()
        {
            Container.BindInstance(HandSettings).AsSingle();
        }
    }
}
