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
    class StatBlockView : NetworkBehaviour, IStatBlockView
    {
        #region Dependencies
        Stone.Settings _stoneSettings;
        [Inject]
        public void Construct(Stone.Settings stoneSettings)
        {
            _stoneSettings = stoneSettings;
        }

        private IStatBlockModel _model;
        public void Initialize(IStatBlockModel model)
        {
            _model = model;
            model.OnStatsChanged += RefreshVisuals;
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
            _statsText.text = _model.StatsStr;
            _statsText.color = _stoneSettings.GetStoneDetails(_model.StoneType).color;
        }
        #endregion
    }
}
