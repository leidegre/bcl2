using System;

namespace Bcl2
{
  public static class BigEndian
  {
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

    public static uint ToUInt32(byte[] bytes, int offset)
    {
      var v = BitConverter.ToUInt32(bytes, offset);
      if (BitConverter.IsLittleEndian)
      {
        v = v.ByteSwap();
      }
      return v;
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
