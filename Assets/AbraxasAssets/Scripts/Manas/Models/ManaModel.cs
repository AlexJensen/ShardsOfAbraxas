using Abraxas.Manas.Controllers;
using Abraxas.Manas.Views;
using Abraxas.Stones;
using Abraxas.Zones.Decks.Controllers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Player = Abraxas.Players.Players;

namespace Abraxas.Manas.Models
{
    class ManaModel : IManaModel
    {
        #region Dependencies
        ManaType.Factory _typeFactory;
        IManaView _view;
        IManaController _controller;
        public ManaModel(ManaType.Factory typeFactory)
        {
            _typeFactory = typeFactory;
        }

        public void Initialize(IManaView view, IManaController controller)
        {
            _view = view;
            _controller = controller;
        }
        #endregion

        #region Properties
        public Player Player { get; set; }
        public List<ManaType> ManaTypes { get => _manaTypes; set => _manaTypes = value; }
        public Dictionary<StoneType, int> DeckCosts { get => _deckCosts; set => _deckCosts = value; }
        public int TotalDeckCost { get => _totalDeckCost; }
        #endregion

        #region Fields
        List<ManaType> _manaTypes;
        Dictionary<StoneType, int> _deckCosts;
        int _totalDeckCost = 0;
        #endregion

        #region Methods
        public void CreateManaTypesFromDeck(IDeckController deck)
        {
            _deckCosts = deck.GetTotalCostOfZone();
            _manaTypes = _deckCosts
                .Where(manaAmount => manaAmount.Value > 0)
                .Select(manaAmount =>
                {
                    ManaType manaType = _typeFactory.Create().GetComponent<ManaType>();
                    manaType.transform.SetParent(_view.Transform);
                    manaType.Mana = _controller;
                    manaType.Type = manaAmount.Key;
                    manaType.Amount = 0;
                    manaType.Player = Player;
                    _totalDeckCost += manaAmount.Value;
                    return manaType;
                })
                .OrderBy(manaType => manaType.Type)
                .ToList();

            for (int i = 0; i < _manaTypes.Count; i++)
            {
                ManaTypes[i].transform.SetSiblingIndex(i);
            }
        }

        public IEnumerator GenerateRatioMana(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                int num = Random.Range(0, _totalDeckCost);
                foreach (KeyValuePair<StoneType, int> manaAmount in _deckCosts)
                {
                    if (manaAmount.Value < num)
                    {
                        num -= manaAmount.Value;
                        continue;
                    }
                    ManaType manaType = ManaTypes.Find(x => x.Type == manaAmount.Key);
                    manaType.Amount += 1;
                    break;
                }
            }
            yield break;
        }
        #endregion
    }
}
