﻿using Abraxas.Cells.Controllers;
using Abraxas.Random.Managers;
using Abraxas.Zones.Fields.Views;
using Abraxas.Zones.Models;
using System.Collections.Generic;

namespace Abraxas.Zones.Fields.Models
{
    class FieldModel : ZoneModel, IFieldModel
    {
        #region Dependencies
        IFieldView _view;
        public override void Initialize<TView>(TView view)
        {
            _view = (IFieldView)view;
            GenerateField();
        }

        public void GenerateField()
        {
            _fieldGrid = _view.GenerateField();
        }
        #endregion

        #region Fields
        List<List<ICellController>> _fieldGrid = new();

        public FieldModel(IRandomManager randomManager) : base(randomManager)
        {
        }
        #endregion

		#region Properties
		public List<List<ICellController>> FieldGrid { get => _fieldGrid; }

        public override ZoneType Type => ZoneType.Field;
        #endregion
    }
}
