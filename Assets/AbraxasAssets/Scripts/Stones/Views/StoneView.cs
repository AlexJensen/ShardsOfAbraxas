using Abraxas.Stones.Models;
using System;
using UnityEngine;

namespace Abraxas.Stones.Views
{
    class StoneView : MonoBehaviour, IStoneView
    {
        IStoneModel _model;
        IStoneController _controller;
        #region Dependencies
        public void Initialize(IStoneModel model, IStoneController controller)
        {
            _model = model;
            _controller = controller;
        }
        #endregion
    }
}
