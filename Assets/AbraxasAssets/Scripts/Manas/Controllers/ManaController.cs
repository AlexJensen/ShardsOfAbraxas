using Abraxas.Cards.Controllers;
using Abraxas.Events;
using Abraxas.Events.Managers;
using Abraxas.Manas.Models;
using Abraxas.Manas.Views;
using Abraxas.Random.Managers;
using Abraxas.Stones;
using Abraxas.Stones.Controllers;
using Abraxas.Zones.Decks.Controllers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Zenject;
using Player = Abraxas.Players.Players;
namespace Abraxas.Manas.Controllers
{

    /// <summary>
    /// ManaController is a class that contains all data to represent a mana gameobject.
    /// </summary>
    class ManaController : IManaController,
        IGameEventListener<Event_LocalPlayerChanged>
    {
        #region Dependencies
        IManaModel _model;
        IManaView _view;

        readonly IRandomManager _randomManager;
        readonly IEventManager _eventManager;
        readonly ManaType.Factory _typeFactory;
        [Inject]
        public ManaController(IRandomManager randomManager, IEventManager eventManager, ManaType.Factory typeFactory)
        {
            _randomManager = randomManager;
            _eventManager = eventManager;
            _typeFactory = typeFactory;

            _eventManager.AddListener(this);
        }
        public void Initialize(IManaView view, IManaModel model)
        {
            _model = model;
            _view = view;

        }

        public class Factory : PlaceholderFactory<IManaView, IManaController>
        {

        }

        public void OnDestroy()
        {
            _eventManager.RemoveListener(this);
        }
        #endregion

        #region Properties
        public Player Player { get => _model.Player; }
        public List<ManaType> ManaTypes { get => _model.ManaTypes; }
        public int StartOfTurnMana { get => _model.StartOfTurnMana; set => _model.StartOfTurnMana = value; }

        #endregion

        public void PurchaseCard(ICardController card)
        {
            foreach (var (cost, result) in from cost in card.TotalCosts
                                           let result = ManaTypes.Find(x => x.Type == cost.Key)
                                           select (cost, result))
            {
                result.Amount -= cost.Value;
            }

            _view.StartManaEventCoroutine(_eventManager.RaiseEvent(new Event_ManaModified(this)));
        }

        public void CreateManaTypesFromDeck(IDeckController deck)
        {
            _model.DeckCosts = deck.GetTotalCostOfZone();
            _model.ManaTypes = _model.DeckCosts
                .Where(manaAmount => manaAmount.Value > 0)
                .Select(manaAmount =>
                {
                    ManaType manaType = _typeFactory.Create().GetComponent<ManaType>();
                    manaType.transform.SetParent(_view.Transform);
                    manaType.Mana = this;
                    manaType.Player = Player;
                    manaType.Initialize();
                    manaType.Type = manaAmount.Key;
                    manaType.Amount = 0;

                    _model.TotalDeckCost += manaAmount.Value;
                    return manaType;
                })
                .OrderBy(manaType => manaType.Type)
                .ToList();

            for (int i = 0; i < _model.ManaTypes.Count; i++)
            {
                ManaTypes[i].transform.SetSiblingIndex(i);
            }
            _view.StartManaEventCoroutine(_eventManager.RaiseEvent(new Event_ManaModified(this)));
        }
        public ManaAmounts GenerateRatioMana(int amount)
        {
            var generatedMana = new ManaAmounts
            {
                ManaTypes = new List<StoneType>(),
                Amounts = new List<int>()
            };

            for (int i = 0; i < amount; i++)
            {
                int num = _randomManager.Range(0, _model.TotalDeckCost);
                foreach (var manaAmount in _model.DeckCosts)
                {
                    if (manaAmount.Value < num)
                    {
                        num -= manaAmount.Value;
                        continue;
                    }

                    var index = generatedMana.ManaTypes.IndexOf(manaAmount.Key);
                    if (index >= 0)
                    {
                        generatedMana.Amounts[index] += 1;
                    }
                    else
                    {
                        generatedMana.ManaTypes.Add(manaAmount.Key);
                        generatedMana.Amounts.Add(1);
                    }
                    break;
                }
            }
            return generatedMana;
        }



        public void ApplyGeneratedMana(ManaAmounts generatedManaAmounts)
        {
            for (int i = 0; i < generatedManaAmounts.ManaTypes.Count; i++)
            {
                var manaType = ManaTypes.Find(x => x.Type == generatedManaAmounts.ManaTypes[i]);
                if (manaType != null)
                {
                    manaType.Amount += generatedManaAmounts.Amounts[i];
                }
            }
            _view.StartManaEventCoroutine(_eventManager.RaiseEvent(new Event_ManaModified(this)));
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

        public bool CanPurchaseStone(IStoneController stone)
        {

            return ManaTypes.Find(x => x.Type == stone.StoneType).Amount >= stone.Cost;
        }

        public void PurchaseStone(IStoneController stone)
        {
            ManaType result = ManaTypes.Find(x => x.Type == stone.StoneType);
            result.Amount -= stone.Cost;
            _view.StartManaEventCoroutine(_eventManager.RaiseEvent(new Event_ManaModified(this)));
        }

        public IEnumerator OnEventRaised(Event_LocalPlayerChanged eventData)
        {
            _model.Player = _view.Player;
            yield break;
        }

        public bool ShouldReceiveEvent(Event_LocalPlayerChanged eventData)
        {
            return true;
        }
    }
}
