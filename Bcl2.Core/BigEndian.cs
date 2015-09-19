using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bcl2
{
  public static class BigEndian
  {
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

    public static ulong ToUInt64(byte[] bytes, int offset)
    {
      var v = BitConverter.ToUInt64(bytes, offset);
      if (BitConverter.IsLittleEndian)
      {
        // for correctness yo!
        // (trivia: Xbox360 is big endian and it can run .NET yay!)
        v = v.ByteSwap();
      }
      return v;
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

    public static uint ToUInt32(byte[] bytes, int offset)
    {
      var v = BitConverter.ToUInt32(bytes, offset);
      if (BitConverter.IsLittleEndian)
      {
        v = v.ByteSwap();
      }
      return v;
    }

    public static ushort ByteSwap(this ushort v)
    {
      return (ushort)(((v << 8) | (v >> 8)) & 0xffff);
    }

    public static ushort ToUInt16(byte[] bytes, int offset)
    {
      var v = BitConverter.ToUInt16(bytes, offset);
      if (BitConverter.IsLittleEndian)
      {
        v = v.ByteSwap();
      }
      return v;
    }
  }
}
