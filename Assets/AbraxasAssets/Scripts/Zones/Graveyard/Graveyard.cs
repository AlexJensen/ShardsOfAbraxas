using Abraxas.Cards;
using System;
using UnityEngine;
using Zenject;

using Player = Abraxas.Players.Players;

namespace Abraxas.Zones.Graveyards
{
    public class Graveyard : Zone
    {
        #region Settings
        Settings _graveyardSettings;
        [Serializable]
        public class Settings
        {
            public float MoveCardToGraveyardTime;
        }
        #endregion

        #region Dependencies
        [Inject]
        public void Construct(Settings graveyardSettings)
        {
            _graveyardSettings = graveyardSettings;
        }
        #endregion

        #region Fields
        [SerializeField]
        Player player;

        #endregion

        #region Properties
        public Player Player { get => player; }
        public override float MoveCardTime => _graveyardSettings.MoveCardToGraveyardTime;
        public override Zones ZoneType => Zones.DECK;
        #endregion
    }
}