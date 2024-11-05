using Abraxas.Conditions.Factories;
using Abraxas.Stones.Conditions;
using Abraxas.Stones.Controllers;
using Abraxas.Stones.Controllers.StoneTypes.Conditions;
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
            Container.BindInterfacesAndSelfTo<Condition_IsCurrentState>().AsTransient();
            Container.BindInterfacesAndSelfTo<Condition_IsTargetPlayerActivePlayer>().AsTransient();
            Container.BindInterfacesAndSelfTo<Condition_IsTargetCardInZone>().AsTransient();
            Container.BindInterfacesAndSelfTo<Condition_Trigger_PlayerDrawsCards>().AsTransient();
            Container.BindInterfacesAndSelfTo<Condition_IsTargetEqualToTarget>().AsTransient();
            Container.BindInterfacesAndSelfTo<Condition_WasTargetCardInZone>().AsTransient();

            Container.Bind<ConditionSOBase>().To(x => x
           .AllNonAbstractClasses()
           .DerivingFrom<ConditionSOBase>())
           .AsTransient();

            // Bind ICondition to all its implementations
            Container.Bind<ICondition>().To(x => x
                .AllNonAbstractClasses()
                .DerivingFrom<ICondition>())
                .AsTransient();

            Container.BindFactory<ConditionSOBase, IStoneController, ICondition, ConditionSOBase.Factory>().FromFactory<ConditionFactory>();
        }
    }
}
