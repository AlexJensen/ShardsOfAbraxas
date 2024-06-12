using Abraxas.Decks.Installers;
using Abraxas.Fields.Installers;
using Abraxas.Graveyards.Installers;
using Abraxas.Hands.Installers;
using Abraxas.Zones.Managers;
using Zenject;

namespace Abraxas.Zones.Installers
{
    public class ZoneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ZoneManager>().AsSingle();
            HandInstaller.Install(Container);
            FieldInstaller.Install(Container);
            GraveyardInstaller.Install(Container);
            DeckInstaller.Install(Container);
        }
    }
}
