using UnityEngine;
using Zenject;

namespace Abraxas.Core.Installers
{
    [CreateAssetMenu(menuName = "Abraxas/Game Settings")]
    class AbraxasSettingsInstaller : ScriptableObjectInstaller<AbraxasSettingsInstaller>
    {
        public AbraxasInstaller.Settings GameInstaller;

        public override void InstallBindings()
        {
            Container.BindInstance(GameInstaller);
        }
    }
}