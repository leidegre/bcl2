using System;

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
    /// Create a new instance of the random class seeded by a cryptographically secure pseudorandom number generator.
    /// </summary>
    public static Random NextRandom()
    {
      var bytes = new byte[4];
      NextSecure(bytes);
      var seed = BitConverter.ToInt32(bytes, 0);
      return new Random(seed);
    }

    public static int Next()
    {
      lock (_rng)
      {
        return _rng.Next();
      }
    }

    public static long Next64(this Random r)
    {
      var a = r.Next();
      var b = r.Next();
      return Math.BigMul(a, b);
    }

    public static long Next64()
    {
      lock (_rng)
      {
        return _rng.Next64();
      }
    }
  }
}
