using Abraxas.Stones;
using System;
using Unity.Netcode;
using UnityEngine;
namespace Abraxas.StatBlocks.Data
{
    /// <summary>
    /// StatBlockData is a serializable struct that contains all data to represent a stat block.
    /// </summary>
    [Serializable]
    public struct StatBlockData : INetworkSerializable
    {
        #region Fields
        public StatData Stats;
        public int Cost;
        public StoneType StoneType;
        #endregion

        #region Properties
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Stats);
            serializer.SerializeValue(ref Cost);
            serializer.SerializeValue(ref StoneType);
        }
        #endregion
    }

    [Serializable]
    public struct StatData :INetworkSerializable
    {
        public int _ATK;
        public int _DEF;
        public int _SPD;
        public int _RNG;

        public int ATK { readonly get => _ATK; set => _ATK = value; }
        public int DEF { readonly get => _DEF; set => _DEF = value; }
        public int SPD { readonly get => _SPD; set => _SPD = value; }
        public int RNG { readonly get => _RNG; set => _RNG = value; }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _ATK);
            serializer.SerializeValue(ref _DEF);
            serializer.SerializeValue(ref _SPD);
            serializer.SerializeValue(ref _RNG);
        }

        public static StatData operator +(StatData a, StatData b)
        {
            return new StatData
            {
                ATK = a.ATK + b.ATK,
                DEF = a.DEF + b.DEF,
                SPD = a.SPD + b.SPD,
                RNG = a.RNG + b.RNG
            };
        }

        public static StatData operator *(StatData a, StatData b)
        {
            return new StatData
            {
                ATK = a.ATK * b.ATK,
                DEF = a.DEF * b.DEF,
                SPD = a.SPD * b.SPD,
                RNG = a.RNG * b.RNG
            };
        }

        public static StatData operator /(StatData a, StatData b)
        {
            return new StatData
            {
                ATK = (int)Math.Ceiling((float)a.ATK / b.ATK),
                DEF = (int)Math.Ceiling((float)a.DEF / b.DEF),
                SPD = (int)Math.Ceiling((float)a.SPD / b.SPD),
                RNG = (int)Math.Ceiling((float)a.RNG / b.RNG)
            };
        }

        public static StatData operator -(StatData a, StatData b)
        {
            return new StatData
            {
                ATK = a.ATK - b.ATK,
                DEF = a.DEF - b.DEF,
                SPD = a.SPD - b.SPD,
                RNG = a.RNG - b.RNG
            };
        }
        public readonly bool Equals(StatData other)
        {
            return ATK.Equals(other.ATK) &&
                DEF.Equals(other.DEF) &&
                SPD.Equals(other.SPD) &&
                RNG.Equals(other.RNG);
        }
    }



}
