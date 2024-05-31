using Abraxas.StatBlocks.Controllers;
using Abraxas.StatBlocks.Models;
using Abraxas.StatBlocks.Views;
using Abraxas.Stones;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Abraxas.StackBlocks.Views
{
    /// <summary>
    /// StatBlockView is a MonoBehaviour that displays the stats of a stat block.
    /// </summary>
    class StatBlockView : NetworkBehaviour, IStatBlockView
    {
        #region Dependencies
        Stone.Settings _stoneSettings;
        [Inject]
        public void Construct(Stone.Settings stoneSettings)
        {
            _stoneSettings = stoneSettings;
        }

        IStatBlockModel _model;
        IStatBlockController _controller;
        public void Initialize(IStatBlockModel model, IStatBlockController controller)
        {
            _model = model;
            _controller = controller;

            model.OnStatsChanged += RefreshVisuals;
            RefreshVisuals();
        }

        public override void OnDestroy()
        {
            _model.OnStatsChanged -= RefreshVisuals;
        }
        #endregion

        #region Fields
        [SerializeField]
        TMP_Text _statsText;
        #endregion

        #region Methods
        private void RefreshVisuals()
        {
            _statsText.text = _controller.StatsStr;
            _statsText.color = _stoneSettings.GetStoneTypeDetails(_controller.StoneType).color;
        }
        #endregion
    }
}
