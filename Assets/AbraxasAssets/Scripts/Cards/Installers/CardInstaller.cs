using Abraxas.Cards.Controllers;
using Abraxas.Cards.Data;
using Abraxas.Cards.Factories;
using Abraxas.Cards.Models;
using Abraxas.Cards.Views;
using Zenject;

namespace Abraxas.Cards.Installers
{
    public class CardInstaller : MonoInstaller
    {
        #region Bindings
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CardView>().AsTransient();
            Container.BindInterfacesAndSelfTo<CardController>().AsTransient();
            Container.BindInterfacesAndSelfTo<CardModel>().AsTransient();
            Container.BindFactory<CardData, ICardController, CardController.Factory>().FromFactory<CardFactory>();
        }
        #endregion
    }
}
