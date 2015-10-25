using System;

namespace Bcl2
{
  /// <summary>
  /// This class contains a collection of methods that use bit manipulation to make intresting things happen faster.
  /// </summary>
  public static class BitTwiddling
  {
    // ByteSwap:

    public static ulong ByteSwap(this ulong v)
    {
      ulong x = (v << 56)
        | ((v >> 0x28) & 0x000000000000ff00UL)
        | ((v >> 0x18) & 0x0000000000ff0000UL)
        | ((v >> 0x08) & 0x00000000ff000000UL)
        | ((v << 0x08) & 0x000000ff00000000UL)
        | ((v << 0x18) & 0x0000ff0000000000UL)
        | ((v << 0x28) & 0x00ff000000000000UL)
        | (v >> 56)
        ;
      return x;
    }

    public static uint ByteSwap(this uint v)
    {
      uint x = (v << 24)
        | ((v >> 8) & 0x0000ff00U)
        | ((v << 8) & 0x00ff0000U)
        | (v >> 24)
        ;
      return x;
    }

    public static ushort ByteSwap(this ushort v)
    {
      return (ushort)(((v << 8) | (v >> 8)) & 0xffff);
    }

    // IsPowerOfTwo:

    public static bool IsPowerOfTwo(int v)
    {
      return IsPowerOfTwo((uint)v);
    }

    public static bool IsPowerOfTwo(uint v)
    {
      if (v == 0)
      {
        throw new ArgumentOutOfRangeException("v", "This function is not defined for zero.");
      }
      return (v & (~v + 1)) == v;
    }

    // LeastCommonMultiple:

    public static int LeastCommonMultiple(int u, int v)
    {
      return (u * v) / GreatestCommonDivisor(u, v);
    }

    /// <summary>
    /// Compute the least (or smallest) common multiple of two integers.
    /// </summary>
    /// <remarks>
    /// This function uses the binary method to compute the GCD under the hood.
    /// </remarks>
    public static uint LeastCommonMultiple(uint u, uint v)
    {
      return (u * v) / GreatestCommonDivisor(u, v);
    }

    // GreatestCommonDivisor:

    public static int GreatestCommonDivisor(int u, int v)
    {
      return (int)GreatestCommonDivisor((uint)Math.Abs(u), (uint)Math.Abs(v));
    }

    /// <summary>
    /// Compute the greatest common divisor (GCD) of two integers using the binary method.
    /// </summary>
    public static uint GreatestCommonDivisor(uint u, uint v)
    {
      // see https://en.wikipedia.org/wiki/Binary_GCD_algorithm

      // simple cases (termination)
      if (u == v)
        return u;

      if (u == 0)
        return v;

      if (v == 0)
        return u;

      // look for factors of 2
      if ((~u & 1) != 0) // u is even
      {
        if ((v & 1) != 0) // v is odd
          return GreatestCommonDivisor(u >> 1, v);
        else // both u and v are even
          return GreatestCommonDivisor(u >> 1, v >> 1) << 1;
      }

      if ((~v & 1) != 0) // u is odd, v is even
        return GreatestCommonDivisor(u, v >> 1);

      // reduce larger argument
      if (u > v)
        return GreatestCommonDivisor((u - v) >> 1, v);

      return GreatestCommonDivisor((v - u) >> 1, u);
    }
  }
}
