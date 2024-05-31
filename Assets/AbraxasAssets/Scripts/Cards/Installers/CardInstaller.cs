using Abraxas.Cards.Controllers;
using Abraxas.Cards.Data;
using Abraxas.Cards.Factories;
using Abraxas.Cards.Managers;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Zenject;

namespace Abraxas.Cards.Installers
{
    /// <summary>
    /// CardInstaller is a Unity-Specific installer for the card system.
    /// </summary>
    public class CardInstaller : MonoInstaller<CardInstaller>
    {
        #region Bindings
        public override void InstallBindings()
        {
            CardBaseInstaller.Install(Container);
        }
        #endregion
    }

    /// <summary>
    /// CardBaseInstaller is a generic installer for the card system used for unit testing.
    /// </summary>
    public class CardBaseInstaller : Installer<CardBaseInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CardManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<CardView>().AsTransient();
            Container.BindInterfacesAndSelfTo<CardController>().AsTransient();
            Container.BindInterfacesAndSelfTo<CardModel>().AsTransient();
            Container.BindFactory<CardData, ICardController, CardController.Factory>().FromFactory<CardFactory>();
        }
    }
}
