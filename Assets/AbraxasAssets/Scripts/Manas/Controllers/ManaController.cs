using Abraxas.Cards.Models;
using Abraxas.Manas.Models;
using Abraxas.Manas.Views;
using Abraxas.Zones.Decks.Controllers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Zenject;
using Player = Abraxas.Players.Players;
namespace Abraxas.Manas.Controllers
{
    class ManaController: IManaController
    {
        #region Dependencies

        IManaView _view;
        IManaModel _model;


        public void Initialize(IManaView view, IManaModel model)
        {
            _view = view;
            _model = model;
            model.Player = view.Player;
        }

        public class Factory : PlaceholderFactory<IManaView, IManaController>
        {

        }
        #endregion

        #region Properties
        public Player Player { get => _model.Player; }
        public List<ManaType> ManaTypes { get => _model.ManaTypes; }
        #endregion

        public void PurchaseCard(ICardModel card)
        {
            foreach (var cost in card.TotalCosts)
            {
                ManaType result = ManaTypes.Find(x => x.Type == cost.Key);
                result.Amount -= cost.Value;
            }
        }

        public void CreateManaTypesFromDeck(IDeckController deck)
        {
            _model.CreateManaTypesFromDeck(deck);
        }
        public IEnumerator GenerateRatioMana(int amount)
        {
            yield return _model.GenerateRatioMana(amount);
        }

        public bool CanPurchaseCard(ICardModel card)
        {
            foreach (var _ in from cost in card.TotalCosts
                              let result = ManaTypes.Find(x => x.Type == cost.Key)
                              where !result || result.Amount < cost.Value
                              select new { })
            {
                return false;
            }

            return true;
        }
    }
}
