using System;
using System.Collections.Generic;

namespace Bcl2
{
  public static class Randomness
  {
    private static readonly System.Security.Cryptography.RNGCryptoServiceProvider _secureRng;
    private static readonly Random _rng;

    static Randomness()
    {
      _secureRng = new System.Security.Cryptography.RNGCryptoServiceProvider();
      _rng = NextRandom();
    }

    /// <summary>
    /// Fills an array of bytes with a cryptographically strong sequence of random values.
    /// </summary>
    public static void NextSecure(byte[] bytes)
    {
      _secureRng.GetBytes(bytes);
    }

    public static double NextSecureDouble()
    {
      var bytes = new byte[8];
      _secureRng.GetBytes(bytes);
      var v = BitConverter.ToUInt64(bytes, 0);
      return (double)v / ((double)ulong.MaxValue + 1); // note: we only use 54-bits of precision here
    }

    /// <summary>
    /// Create a new instance of the random class seeded by a cryptographically secure pseudorandom number generator.
    /// </summary>
    public static Random NextRandom()
    {
      var bytes = new byte[4];
      NextSecure(bytes);
      var seed = BitConverter.ToInt32(bytes, 0);
      return new Random(seed);
    }

    public static IList<T> NextList<T>(this Random r, IEnumerable<T> source)
    {
      var list = new List<T>();
      foreach (var item in source)
      {
        var i = r.Next(list.Count + 1);
        if (i == list.Count)
        {
          list.Add(item);
        }
        else
        {
          var temp = list[i];
          list[i] = item;
          list.Add(temp);
        }
      }
      return list;
    }

    public static T NextElement<T>(this Random r, T[] source)
    {
      var index = r.Next(source.Length);
      var v = source[index];
      return v;
    }

    internal static IEnumerable<byte> NextByteSequence(this Random r)
    {
      for (;;)
      {
        var v = (byte)r.Next(byte.MaxValue + 1);
        yield return v;
      }
    }

    internal static IEnumerable<ushort> NextUInt16Sequence(this Random r)
    {
      for (;;)
      {
        var v = (ushort)r.Next(ushort.MaxValue + 1);
        yield return v;
      }
    }

    public static IEnumerable<int> NextInt32Sequence(this Random r)
    {
      for (;;)
      {
        var v = int.MinValue + 2 * r.NextDouble() * int.MaxValue;
        yield return (int)v;
      }
    }

    public static IEnumerable<uint> NextUInt32Sequence(this Random r)
    {
      for (;;)
      {
        var v = r.NextDouble() * uint.MaxValue;
        yield return (uint)v;
      }
    }

    public static IEnumerable<long> NextInt64Sequence(this Random r)
    {
      for (;;)
      {
        var v = long.MinValue + 2 * r.NextDouble() * long.MaxValue;
        yield return (long)v;
      }
    }

    public static IEnumerable<ulong> NextUInt64Sequence(this Random r)
    {
      for (;;)
      {
        var v = r.NextDouble() * ulong.MaxValue;
        yield return (ulong)v;
      }
    }
  }
}
