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
        #region Dependencies
        Card.Settings _cardSettings;
        [Inject]
        public void Construct(Card.Settings cardSettings)
        {
            _cardSettings = cardSettings;
        }
        #endregion

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
