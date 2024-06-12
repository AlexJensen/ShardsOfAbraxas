using UnityEngine;
using Zenject;

namespace Abraxas.Players.Installers
{
    /// <summary>
    /// PlayerSettingsInstaller is a ScriptableObjectInstaller that installs PlayerSettings into the Zenject container.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerSettings", menuName = "Abraxas/Settings/Player Settings")]
    class PlayerSettingsInstaller : ScriptableObjectInstaller<PlayerSettingsInstaller>
    {
        public Player.Settings PlayerSettings;
        public override void InstallBindings()
        {
            Container.BindInstance(PlayerSettings).AsSingle();
        }
    }
}
