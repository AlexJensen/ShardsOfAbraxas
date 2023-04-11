using Abraxas.Behaviours.Data;
using Abraxas.Behaviours.Players;
using Abraxas.Behaviours.Zones.Decks;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Abraxas.Behaviours.Manas
{
    public class Mana : MonoBehaviour
    {
        #region Dependency Injections
        ManaType.Factory _typeFactory;
        [Inject]
        void Construct(ManaType.Factory typeFactory)
        {
            _typeFactory = typeFactory;
        }
        #endregion

        #region Fields
        [SerializeField]
        Deck _deck;

        [SerializeField]
        Player _player;

        Dictionary<StoneData.StoneType, int> _deckCosts;
        List<ManaType> _manaTypes;
        int _totalDeckCost = 0;
        #endregion


        #region Properties
        public Player Player { get => _player; }
        public List<ManaType> ManaTypes { get => _manaTypes; }
        #endregion

        #region Unity Methods
        void Start()
        {
            _deckCosts = _deck.GetTotalDeckCosts();
            _manaTypes = new List<ManaType>();
            foreach (KeyValuePair<StoneData.StoneType, int> manaAmount in _deckCosts)
            {
                if (manaAmount.Value > 0)
                {
                    ManaType manaType = _typeFactory.Create().GetComponent<ManaType>();
                    manaType.transform.SetParent(transform);
                    manaType.Type = manaAmount.Key;
                    manaType.Amount = 0;
                    manaType.Player = _player;
                    _totalDeckCost += manaAmount.Value;
                    ManaTypes.Add(manaType);
                }
            }

            _manaTypes.Sort((a, b) => a.Type.CompareTo(b.Type));
            for (int i = 0; i < _manaTypes.Count; i++)
            {
                _manaTypes[i].transform.SetSiblingIndex(i);
            }
        }
        #endregion

        #region Methods
        public void GenerateRatioMana(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                int num = Random.Range(0, _totalDeckCost);
                foreach (KeyValuePair<StoneData.StoneType, int> manaAmount in _deckCosts)
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
        }
        #endregion
    }
}