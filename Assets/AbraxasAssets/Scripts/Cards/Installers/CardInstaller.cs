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

        }
        #endregion
    }
}
