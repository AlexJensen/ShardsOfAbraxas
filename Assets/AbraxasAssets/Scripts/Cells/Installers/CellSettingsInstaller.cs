using UnityEngine;
using Zenject;

namespace Abraxas.Cells.Installers
{
    [CreateAssetMenu(fileName = "CellSettings", menuName = "Abraxas/Settings/Cell Settings")]
    class CellSettingsInstaller : ScriptableObjectInstaller<CellSettingsInstaller>
    {
        public Cell.Settings CellSettings;
        public override void InstallBindings()
        {
            Container.BindInstance(CellSettings).AsSingle();
        }
    }
}
