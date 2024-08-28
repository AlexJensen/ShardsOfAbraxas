using Abraxas.Stones.Controllers;
using Abraxas.Stones.Targets;
using Abraxas.Targets.Factories;
using Zenject;

namespace Abraxas.Targets.Installers
{
    internal class TargetInstaller : MonoInstaller<TargetInstaller>
    {
        #region Bindings

        public override void InstallBindings()
        {
            Container.BindFactory<TargetSOBase, IStoneController, ITarget, TargetSOBase.Factory>().FromFactory<TargetFactory>();
        }
        #endregion
    }
}
