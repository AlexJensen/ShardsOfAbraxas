using Abraxas.Cards.Controllers;
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
        IManaModel _model;


        public void Initialize(IManaView view, IManaModel model)
        {
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

        public void PurchaseCard(ICardController card)
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

        public bool CanPurchaseCard(ICardController card)
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
