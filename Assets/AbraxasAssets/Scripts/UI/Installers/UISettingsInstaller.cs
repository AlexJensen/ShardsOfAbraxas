using UnityEngine;
using Zenject;

namespace Abraxas.UI.Installers
{
    [CreateAssetMenu(fileName = "UISettings", menuName = "Abraxas/UI Settings")]
    class UISettingsInstaller : ScriptableObjectInstaller<UISettingsInstaller>
    {
        public NextButton.Settings NextButtonSettings;
        public override void InstallBindings()
        {
            Container.BindInstance(NextButtonSettings).AsSingle();
        }
    }
}
