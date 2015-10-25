using System;

namespace Bcl2.PositionalNotation
{
  /// <summary>
  /// Represents a mutable arbitrary precision unsigned integer for use with big number coding.
  /// </summary>
  /// <remarks>
  /// This class has been written specifically for the big number coding scheme 
  /// and supports methods in a way that makes sense for the big number coding scheme only.
  /// </remarks>
  public sealed class BigInt
  {
    private readonly uint[] _num;
    private readonly int _byteCount;

    public int ByteCount { get { return _byteCount; } }

    public BigInt(uint v)
    {
      this._num = new[] { v.ByteSwap() };
      this._byteCount = 4;
    }

    public BigInt(ulong v)
    {
      var x = v.ByteSwap();
      this._num = new[] { (uint)(x >> 32), (uint)x };
      this._byteCount = 8;
    }

    public BigInt(byte[] bytes, int offset, int count)
    {
      var num = new uint[(count + 3) / 4];

      // Load number in reverse byte order (big endian).
      uint x = 0;

      int i = 0;
      int j = 0;
      int k = 0;
      for (i = offset + count - 1; i >= offset; i--)
      {
        if ((j > 0) & ((j & 3) == 0))
        {
          num[k++] = x;
          x = 0; // reset
        }
        var bitShift = (j & 3) << 3;
        x |= (uint)bytes[i] << bitShift;
        j++;
      }
      if (x > 0)
      {
        num[k++] = x;
      }

      this._num = num;
      this._byteCount = count;
    }

    /// <summary>
    /// Create a empty (zero filled) integer with a capacity given by <paramref name="byteCount"/>.
    /// </summary>
    public static BigInt FromByteCount(int byteCount)
    {
      return new BigInt(byteCount);
    }

    private BigInt(int byteCount)
    {
      _num = new uint[(byteCount + 3) / 4];
      _byteCount = byteCount;
    }

    public uint DivRem(uint x)
    {
      ulong uu = 0;
      for (int i = _num.Length - 1; i >= 0; i--)
      {
        uu = Pack(uu, _num[i]);
        _num[i] = (uint)(uu / x);
        uu %= x;
      }
      return (uint)uu;
    }

    private static ulong Pack(ulong hi, uint lo)
    {
      return (hi << 32) | lo;
    }

    public void Mul(uint x)
    {
      uint carry = 0;
      for (int i = 0; i < _num.Length; i++)
      {
        carry = MulWithCarry(ref _num[i], x, carry);
      }
      if (carry > 0)
      {
        throw new OverflowException("This implementation will not resize as needed.");
      }
    }

    private static uint MulWithCarry(ref uint num, uint x, uint carry)
    {
      // This is guaranteed not to overflow.
      ulong r = (ulong)num * x + carry;
      num = (uint)r;
      return (uint)(r >> 32);
    }

    public void Add(uint x)
    {
      int i = 0;
      uint carry = Add(ref _num[i++], x);
      if (carry > 0)
      {
        while ((carry = Add(ref _num[i++], carry)) > 0)
          ;
      }
      if (carry > 0)
      {
        throw new OverflowException("This implementation will not resize as needed.");
      }
    }

    private static uint Add(ref uint num, uint x)
    {
      ulong r = (ulong)num + x;
      num = (uint)r;
      return (uint)(r >> 32);
    }

    public byte[] ToByteArray()
    {
      var bytes = new byte[_byteCount];
      Buffer.BlockCopy(_num, 0, bytes, 0, bytes.Length);
      Array.Reverse(bytes);
      return bytes;
    }
  }
}
