using Abraxas.Stones;
using System;
using Unity.Netcode;
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
        public int ATK;
        public int DEF;
        public int SPD;
        public int RNG;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ATK);
            serializer.SerializeValue(ref DEF);
            serializer.SerializeValue(ref SPD);
            serializer.SerializeValue(ref RNG);
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
        public bool Equals(StatData other)
        {
            return ATK.Equals(other.ATK) &&
                DEF.Equals(other.DEF) &&
                SPD.Equals(other.SPD) &&
                RNG.Equals(other.RNG);
        }
    }



}
