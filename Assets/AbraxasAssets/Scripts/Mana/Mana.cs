using Abraxas.Behaviours.Data;
using Abraxas.Behaviours.Game;
using Abraxas.Behaviours.Zones.Decks;
using System.Collections.Generic;
using UnityEngine;

namespace Abraxas.Behaviours.Manas
{
    public class Mana : MonoBehaviour
    {
        [SerializeField]
        Deck deck;

        [SerializeField]
        GameObject manaType;

        [SerializeField]
        GameManager.Player player;

        Dictionary<StoneData.StoneType, int> deckCosts;
        List<ManaType> manaTypes;
        int totalDeckCost = 0;

        public GameManager.Player Player { get => player; }
        public List<ManaType> ManaTypes { get => manaTypes; }

        void Start()
        {
            deckCosts = deck.GetTotalDeckCosts();
            manaTypes = new List<ManaType>();
            foreach (KeyValuePair<StoneData.StoneType, int> manaAmount in deckCosts)
            {
                if (manaAmount.Value > 0)
                {
                    ManaType manaType = Instantiate(this.manaType, transform).GetComponent<ManaType>();
                    manaType.Type = manaAmount.Key;
                    manaType.Amount = 0;
                    manaType.Player = player;
                    totalDeckCost += manaAmount.Value;
                    ManaTypes.Add(manaType);
                }
            }
            // Sort the mana types by their stone type
            manaTypes.Sort((a, b) => a.Type.CompareTo(b.Type));

            // Update the sibling indexes of the mana type game objects
            for (int i = 0; i < manaTypes.Count; i++)
            {
                manaTypes[i].transform.SetSiblingIndex(i);
            }
        }

        /// <summary>
        /// Produce mana randomly based on the starting mana values present in the deck at the start of the game.
        /// The type of mana selected depends on how common that type is present in the deck.
        /// </summary>
        /// <param name="amount">Amount of mana to generate.</param>
        public void GenerateRatioMana(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                int num = Random.Range(0, totalDeckCost);
                foreach (KeyValuePair<StoneData.StoneType, int> manaAmount in deckCosts)
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
    }
}