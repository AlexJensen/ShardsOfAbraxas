using UnityEngine;
using Zenject;

namespace Abraxas.Players.Installers
{
    [CreateAssetMenu(fileName = "PlayerSettings", menuName = "Abraxas/Player Settings")]
    class PlayerSettingsInstaller : ScriptableObjectInstaller<PlayerSettingsInstaller>
    {
        public Player.Settings PlayerSettings;
        public override void InstallBindings()
        {
            Container.BindInstance(PlayerSettings).AsSingle();
        }
    }
}
