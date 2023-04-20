using Abraxas.Cards;
using Abraxas.Players;
using UnityEngine;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Graveyards
{
    public class Graveyard : Zone
    {
        #region Settings
        Settings _settings;
        [SerializeField]
        public class Settings
        {
            public float MoveCardToGraveyardTime;
        }

        #endregion
        #region Fields
        [SerializeField]
        Player player;
        public override Zones ZoneType => Zones.DECK;
        #endregion

        #region Properties
        public Player Player { get => player; }
        public override float MoveCardTime => _settings.MoveCardToGraveyardTime;
        #endregion

        #region Methods
        public void AddCard(Card card, int index = 0)
        {
            card.transform.localScale = Vector3.zero;
            card.transform.position = transform.position;
            card.Zone = Zones.GRAVEYARD;
            card.transform.parent = Cards.transform;
            card.transform.SetSiblingIndex(index);
        }
        public Card RemoveCard(int index = 0)
        {
            Card card = Cards.GetChild(index).GetComponent<Card>();
            card.transform.localScale = Vector3.one;
            card.transform.position = transform.position;
            return card;
        }
        #endregion
    }
}