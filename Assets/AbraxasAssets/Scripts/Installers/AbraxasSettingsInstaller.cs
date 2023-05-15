using Abraxas.Cards;
using Abraxas.Manas;
using Abraxas.Players;
using Abraxas.Stones;
using Abraxas.UI;
using Abraxas.Zones.Hands;
using UnityEngine;
using Zenject;

namespace Abraxas.Core.Installers
{
    [CreateAssetMenu(menuName = "Abraxas/Game Settings")]
    class AbraxasSettingsInstaller : ScriptableObjectInstaller<AbraxasSettingsInstaller>
    {
        public Game.Game.Settings GameSettings;
        public Stone.Settings StoneSettings;
        public Card.Settings CardSettings;
        public CardPlaceholder.Settings CardPlaceholderSettings;
        public Player.Settings PlayerSettings;
        public Mana.Settings ManaSettings;
        public NextButton.Settings NextButtonSettings;
        public override void InstallBindings()
        {
            Container.BindInstance(GameSettings).AsSingle();
            Container.BindInstance(CardSettings).AsSingle();
            Container.BindInstance(CardPlaceholderSettings).AsSingle();
            Container.BindInstance(StoneSettings).AsSingle();
            Container.BindInstance(PlayerSettings).AsSingle();
            Container.BindInstance(ManaSettings).AsSingle();
            Container.BindInstance(NextButtonSettings).AsSingle();
        }
    }
}