using UnityEngine;
using UnityEngine.UI;
using Zenject;

using Player = Abraxas.Players.Players;

namespace Abraxas.Menus
{
    class GameOverPopup : Menu
    {
        Players.Player.Settings _playerSettings;
        [Inject]
        public void Construct(Players.Player.Settings playerSettings)
        {
            _playerSettings = playerSettings;
        }

        [SerializeField] private TMPro.TextMeshProUGUI _winnerText;
        [SerializeField] private Image _bgImage;

        internal void SetWinner(Player player)
        {
            var playerDetails = _playerSettings.GetPlayerDetails(player);
            _winnerText.text = playerDetails.name + " Was Victorious!";
            _bgImage.color = playerDetails.color;
        }
    }
}
