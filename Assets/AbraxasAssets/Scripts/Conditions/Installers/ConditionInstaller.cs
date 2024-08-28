using Abraxas.Conditions.Factories;
using Abraxas.Events;
using Abraxas.Stones.Conditions;
using Abraxas.Stones.Controllers;
using Zenject;

namespace Abraxas.Conditions.Installers
{
    /// <summary>
    /// CardInstaller is a Unity-Specific installer for the card system.
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
    /// CardBaseInstaller is a generic installer for the card system used for unit testing.
    /// </summary>
    public class ConditionBaseInstaller : Installer<ConditionBaseInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindFactory<ConditionSO<IEvent>, IStoneController, ICondition, ConditionSOBase.Factory>().FromFactory<ConditionFactory>();
        }
    }
}
