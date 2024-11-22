using Abraxas.Conditions.Factories;
using Abraxas.Stones.Conditions;
using Abraxas.Stones.Controllers;
using Zenject;

namespace Abraxas.Conditions.Installers
{
    /// <summary>
    /// ConditionInstaller is a Unity-Specific installer for conditions.
    /// </summary>
    public class ConditionInstaller : MonoInstaller<ConditionInstaller>
    {
        #region Bindings
        public override void InstallBindings()
        {
            ConditionBaseInstaller.Install(Container);
        }
        #endregion
    }

    /// <summary>
    /// ConditionBaseInstaller is a generic installer for conditions used for unit testing.
    /// </summary>
    public class ConditionBaseInstaller : Installer<ConditionBaseInstaller>
    {
        public override void InstallBindings()
        {

            Container.BindInterfacesAndSelfTo<Condition_IsTargetEqualToTarget>().AsTransient();

            Container.Bind<ConditionSO>().To(x => x
           .AllNonAbstractClasses()
           .DerivingFrom<ConditionSO>())
           .AsTransient();

            // Bind ICondition to all its implementations
            Container.Bind<ICondition>().To(x => x
                .AllNonAbstractClasses()
                .DerivingFrom<ICondition>())
                .AsTransient();

            Container.BindFactory<ConditionSO, IStoneController, ICondition, ConditionSO.Factory>().FromFactory<ConditionFactory>();
        }
    }
}
