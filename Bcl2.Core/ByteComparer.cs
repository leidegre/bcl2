using System;
using System.Collections;
using System.Collections.Generic;

namespace Bcl2
{
  public class ByteComparer : IComparer<byte[]>, IEqualityComparer<byte[]>
  {
    private static int MemoryCompare(byte[] x, byte[] y, int size)
    {
      int c = 0;
      for (int i = 0; i < size; i++)
      {
        c = (x[i] - y[i]);
        if (c != 0)
          break;
      }
      return c;
    }

    public int Compare(byte[] x, byte[] y)
    {
      var minLength = Math.Min(x.Length, y.Length);
      var c = MemoryCompare(x, y, minLength);
      if (c == 0)
      {
        if (x.Length < y.Length)
        {
          c = -1;
        }
        else if (x.Length > y.Length)
        {
          c = 1;
        }
      }
      return c;
    }

    public bool Equals(byte[] x, byte[] y)
    {
      return Compare(x, y) == 0;
    }

    public int GetHashCode(byte[] obj)
    {
      // This is the FNV-1a hash.
      // See https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function#FNV-1a_hash

      uint hashCode = 0;

      for (int i = 0, len = obj.Length; i < len; i++)
      {
        hashCode ^= obj[i];
        hashCode += (hashCode << 1) + (hashCode << 4) + (hashCode << 7) + (hashCode << 8) + (hashCode << 24);
      }

      return (int)hashCode;
    }
  }
}
