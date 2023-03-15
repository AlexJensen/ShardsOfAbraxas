using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mana : MonoBehaviour
{
    [SerializeField]
    Deck deck;

    [SerializeField]
    GameObject p_manaType;

    [SerializeField]
    Game.Player player;

    Dictionary<StoneData.StoneType, int> deckCosts;
    List<ManaType> manaTypes;
    int totalDeckCost = 0;

    public Game.Player Player { get => player; }

    // Start is called before the first frame update
    void Start()
    {
        deckCosts = deck.GetTotalDeckCosts();
        manaTypes = new List<ManaType>();
        foreach (KeyValuePair<StoneData.StoneType, int> manaAmount in deckCosts)
        {
            if (manaAmount.Value > 0)
            {
                ManaType manaType = Instantiate(p_manaType, transform).GetComponent<ManaType>();
                manaType.Type = manaAmount.Key;
                manaType.Amount = 0;
                totalDeckCost += manaAmount.Value;
                manaTypes.Add(manaType);
            }
        }
    }

    public void GenerateRatioMana(int amount)
    {
        int num = Random.Range(0, totalDeckCost);
        foreach (KeyValuePair<StoneData.StoneType, int> manaAmount in deckCosts)
        {
            if (manaAmount.Value < num)
            {
                num -= manaAmount.Value;
                continue;
            }
            ManaType manaType = manaTypes.Find(x => x.Type == manaAmount.Key);
            manaType.Amount += 1;
            break;
        }
        if (amount > 1)
        {
            GenerateRatioMana(amount - 1);
        }
    }
}
