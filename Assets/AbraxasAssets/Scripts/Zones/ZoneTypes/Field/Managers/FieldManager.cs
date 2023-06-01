using System.Collections;
using UnityEngine;
using Unity.Netcode;
using Abraxas.Cards.Controllers;
using System.Drawing;
using Abraxas.Zones.Fields.Views;
using Zenject;
using Abraxas.Cells.Controllers;
using Abraxas.Cells.Views;
using Abraxas.Zones.Fields.Controllers;
using Abraxas.Zones.Fields.Models;
using Abraxas.Zones.Factories;

namespace Abraxas.Zones.Fields.Managers
{
    public class FieldManager : NetworkBehaviour, IFieldManager
    {

        #region Dependencies
        [SerializeField]
        FieldView _fieldView;
        IFieldController _field;

        [Inject]
        void Construct(ZoneFactory<IFieldView, FieldController, FieldModel> fieldFactory, CellController.Factory cellFactory)
        {
            foreach (var cellView in FindObjectsOfType<CellView>())
            {
                cellFactory.Create(cellView);
            }
            _field = fieldFactory.Create(_fieldView);
        }
        #endregion

        #region Methods
        public void AddCard(ICardController card, Point fieldPos)
        {
            card.Zone = _field;
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

        public IEnumerator CombatMovement(ICardController card, Point movement)
        {
            yield return _field.CombatMovement(card, movement);
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