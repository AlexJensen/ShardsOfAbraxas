﻿using Abraxas.Cards.Controllers;
using Abraxas.StatBlocks.Data;
using Abraxas.Stones.Data;
using Abraxas.Stones.Models;
using Abraxas.Stones.Targets;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Abraxas.Stones.Controllers
{
    public class Effect_ModifyTargetCardGroupStatblock : EffectStone, ITargetable<List<ICardController>>
    {
        #region Dependencies
        public override void Initialize(IStoneModel model, ICardController card)
        {
            if (model.Data is ModifyCardStatBlockData data)
            {
                _modification = data.Modification;
            }
            else
            {
                throw new InvalidOperationException("Model data is not of type ModifyCardStatBlockData.");
            }
            base.Initialize(model, card);
        }
        #endregion

        #region Fields
        TargetSO<List<ICardController>> _targets;
        StatData _modification;
        #endregion

        #region Properties
        TargetSO<List<ICardController>> ITargetable<List<ICardController>>.Target { get => _targets; set => _targets = value; }
        #endregion

        #region Methods
        protected override IEnumerator PerformEffect(object[] vals)
        {
            foreach (var target in _targets.Target)
            {
                target.StatBlock.Stats += _modification;
            }

            yield break;
        }
        #endregion
    }
}