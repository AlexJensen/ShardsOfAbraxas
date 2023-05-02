using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Abraxas.Stones;
using Abraxas.Cards;
using System.Linq;

using Player = Abraxas.Players.Players;
using Random = UnityEngine.Random;
using Abraxas.Zones.Decks;

namespace Abraxas.Manas
{
    public class Mana : MonoBehaviour
    {
        #region Settings
        [Serializable]
        public class Settings
        {
            public GameObject ManaTypePrefab;
            public int StartingMana;
            public int ManaPerTurnIncrement;
        }
        #endregion

        #region Dependencies
        ManaType.Factory _typeFactory;
        IDeckManager _deckManager;
        [Inject]
        public void Construct(ManaType.Factory typeFactory, IDeckManager deckManager)
        {
            _typeFactory = typeFactory;
            _deckManager = deckManager;
        }
        #endregion

        #region Fields
        [SerializeField]
        Player _player;
        Dictionary<StoneType, int> _deckCosts;
        List<ManaType> _manaTypes;
        int _totalDeckCost = 0;
        #endregion

        #region Properties
        public Player Player { get => _player; }
        public List<ManaType> ManaTypes { get => _manaTypes; }
        #endregion

        #region Methods
        void Start()
        {
            CreateManaTypes();
        }

        private void CreateManaTypes()
        {
            _deckCosts = _deckManager.GetDeckCost(Player);
            _manaTypes = _deckCosts
                .Where(manaAmount => manaAmount.Value > 0)
                .Select(manaAmount =>
                {
                    ManaType manaType = _typeFactory.Create().GetComponent<ManaType>();
                    manaType.transform.SetParent(transform);
                    manaType.Mana = this;
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
                _manaTypes[i].transform.SetSiblingIndex(i);
            }
        }

        public void PurchaseCard(Card card)
        {
            foreach (var cost in card.TotalCosts)
            {
                ManaType result = ManaTypes.Find(x => x.Type == cost.Key);
                result.Amount -= cost.Value;
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

        public bool CanPurchaseCard(Card card)
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
        #endregion
    }
}