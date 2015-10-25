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

    /// <summary>
    /// Generate a random floating-point number that is greater than or equal to 0.0, and less than 1.0.
    /// </summary>
    public static double NextSecureDouble()
    {
      var bytes = new byte[8];
      _secureRng.GetBytes(bytes);
      var v = BitConverter.ToUInt64(bytes, 0);
      // We only use the 53-bits of integer precision available in a IEEE 754 64-bit double.
      // The result is a fraction, r = (0, 9007199254740991) / 9007199254740992 where 0 <= r && r < 1.
      v &= ((1UL << 53) - 1);
      var r = (double)v / (double)(1UL << 53);
      return r;
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

    /// <summary>
    /// Create a random permutation of the elements in the input sequence.
    /// </summary>
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

    /// <summary>
    /// Pick an element by random from an array.
    /// </summary>
    public static T NextElement<T>(this Random r, T[] source)
    {
      var index = r.Next(source.Length);
      var v = source[index];
      return v;
    }

    /// <summary>
    /// Pick an element by random from a list.
    /// </summary>
    public static T NextElement<T>(this Random r, IList<T> source)
    {
      var index = r.Next(source.Count);
      var v = source[index];
      return v;
    }

    /// <summary>
    /// Generate an infinite sequence of random 32-bit signed integers.
    /// </summary>
    public static IEnumerable<int> NextInt32Sequence(this Random r)
    {
      var bytes = new byte[4];
      for (;;)
      {
        r.NextBytes(bytes);
        var i = BitConverter.ToInt32(bytes, 0);
        yield return i;
      }
    }

    /// <summary>
    /// Generate an infinite sequence of random 32-bit unsigned integers.
    /// </summary>
    public static IEnumerable<uint> NextUInt32Sequence(this Random r)
    {
      var bytes = new byte[4];
      for (;;)
      {
        r.NextBytes(bytes);
        var u = BitConverter.ToUInt32(bytes, 0);
        yield return u;
      }
    }

    /// <summary>
    /// Generate an infinite sequence of random 64-bit signed integers.
    /// </summary>
    public static IEnumerable<long> NextInt64Sequence(this Random r)
    {
      var bytes = new byte[8];
      for (;;)
      {
        r.NextBytes(bytes);
        var i = BitConverter.ToInt64(bytes, 0);
        yield return i;
      }
    }

    /// <summary>
    /// Generate an infinite sequence of random 64-bit unsigned integers.
    /// </summary>
    public static IEnumerable<ulong> NextUInt64Sequence(this Random r)
    {
      var bytes = new byte[8];
      for (;;)
      {
        r.NextBytes(bytes);
        var u = BitConverter.ToUInt64(bytes, 0);
        yield return u;
      }
    }
  }
}
