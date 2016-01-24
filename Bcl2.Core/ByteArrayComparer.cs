using System;
using System.Collections.Generic;
#if WIN32
using System.Runtime.InteropServices;
#endif

namespace Bcl2
{
  public class ByteArrayComparer : IComparer<byte[]>, IEqualityComparer<byte[]>
  {
#if WIN32
    static class NativeMethods
    {
      [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
      public static extern int memcmp(byte[] x, byte[] y, UIntPtr size);
    }
#endif

    private static int MemoryCompare(byte[] x, byte[] y, int size)
    {
#if WIN32
      var minLength = Math.Min(x.Length, y.Length);
      if (!((0 <= size) & (size <= minLength)))
      {
        throw new ArgumentOutOfRangeException("size");
      }
      var c = NativeMethods.memcmp(x, y, new UIntPtr((uint)size));
      return c;
#else
      int c = 0;
      for (int i = 0; i < size; i++)
      {
        c = (x[i] - y[i]);
        if (c != 0)
          break;
      }
      return c;
#endif
    }

    public static int Compare(byte[] x, byte[] y)
    {
      // This memory compare version is used since it's stable
      // we always want to inspect the min length to determine
      // in which way they compare to each other.
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
          c = +1;
        }
      }
      return c;
    }

    int IComparer<byte[]>.Compare(byte[] x, byte[] y)
    {
      var c = Compare(x, y);
      return c;
    }

    public static bool Equals(byte[] x, byte[] y)
    {
      return Compare(x, y) == 0;
    }

    bool IEqualityComparer<byte[]>.Equals(byte[] x, byte[] y)
    {
      return Equals(x, y);
    }

    public static int GetHashCode(byte[] obj)
    {
      // This is the FNV-1a hash.
      // See https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function#FNV-1a_hash

      uint hashCode = 2166136261;

      for (int i = 0, len = obj.Length; i < len; i++)
      {
        hashCode ^= obj[i];
        hashCode *= 16777619;
      }

      return (int)hashCode;
    }

    int IEqualityComparer<byte[]>.GetHashCode(byte[] obj)
    {
      return GetHashCode(obj);
    }
  }
}
