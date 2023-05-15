using System.Collections;
using UnityEngine;
using Unity.Netcode;
using Abraxas.Cards.Controllers;
using System.Drawing;
using Abraxas.Zones.Fields.Controllers;

namespace Abraxas.Zones.Fields.Managers
{
    public class FieldManager : NetworkBehaviour, IFieldManager
    {
        #region Fields
        [SerializeField]
        IFieldController _field;
        #endregion

        #region Methods
        public void AddCard(ICardController card, Point fieldPos)
        {
            _field.AddCard(card, fieldPos);
        }

        public void RemoveCard(ICardController card)
        {
            _field.RemoveCard(card);
        }

        public IEnumerator StartCombat()
        {
            yield return _field.StartCombat();
        }

        public IEnumerator MoveCardAndFight(ICardController card, Point movement)
        {
            yield return _field.MoveCardAndFight(card, movement);
        }

        public IEnumerator MoveCardToCell(ICardController card, Point fieldPos)
        {
            yield return _field.MoveCardToCell(card, fieldPos);
        }

        public PointF GetCellDimensions()
        {
            return _field.GetDefaultCellDimensions();
        }
        #endregion
    }
}