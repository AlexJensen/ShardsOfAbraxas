using UnityEngine;
using Zenject;

namespace Abraxas.Cards.Installers
{
    [CreateAssetMenu(fileName = "CardSettings", menuName = "Abraxas/Card Settings")]
    class CardSettingsInstaller : ScriptableObjectInstaller<CardSettingsInstaller>
    {
        public Card.Settings CardSettings;
        public override void InstallBindings()
        {
            Container.BindInstance(CardSettings).AsSingle();
        }
    }
}
