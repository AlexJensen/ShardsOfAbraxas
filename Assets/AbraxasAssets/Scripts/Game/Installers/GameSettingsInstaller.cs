using UnityEngine;
using Zenject;

namespace Abraxas.Games.Installers
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Abraxas/Settings/Game Settings")]
    class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
    {
        public Game.Settings GameSettings;
        public override void InstallBindings()
        {
            Container.BindInstance(GameSettings).AsSingle();
        }
    }
}
