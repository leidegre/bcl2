using Bcl2.MemoryManagement;
using Bcl2.PositionalNotation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bcl2
{
  public static class BaseConvert
  {
    /// <summary>
    /// Base 2.
    /// </summary>
    public static BaseConverter Binary { get; private set; }

    /// <summary>
    /// Base 10.
    /// </summary>
    public static BaseConverter Decimal { get; private set; }

    /// <summary>
    /// Base 16. Not case sensitive.
    /// </summary>
    public static BaseConverter Hexadecimal { get; private set; }

    /// <summary>
    /// Base 32. Not case sensitive. Triacontakaidecimal just one of many designs for base 32. This extends the hexadecimal base in a natural way using the characters 0123456789ABCDEFGHIJKLMNOPQRSTUV.
    /// </summary>
    public static BaseConverter Triacontakaidecimal { get; private set; }

    /// <summary>
    /// Base 32. Not case sensitive. Crockford's base 32. For expressing numbers in a form that can be conveniently and accurately transmitted between humans and computer systems.
    /// </summary>
    /// <remarks>
    /// See http://www.crockford.com/wrmg/base32.html
    /// </remarks>
    public static BaseConverter Crockford { get; private set; }

    /// <summary>
    /// Base 62. Case sensitive.
    /// </summary>
    public static BaseConverter Base62 { get; private set; }

    static BaseConvert()
    {
      Binary = new BaseConverter("Binary", new Alphabet("01"));
      Decimal = new BaseConverter("Decimal", new Alphabet("0123456789"));
      Hexadecimal = new BaseConverter("Hexadecimal", new Alphabet("0123456789ABCDEF", "abcdef", "ABCDEF"));
      Triacontakaidecimal = new BaseConverter("Triacontakaidecimal", new Alphabet("0123456789ABCDEFGHIJKLMNOPQRSTUV", "abcdefghijklmnopqrstuv", "ABCDEFGHIJKLMNOPQRSTUV"));
      Crockford = new BaseConverter("Crockford", new Alphabet("0123456789ABCDEFGHJKMNPQRSTVWXYZ", "OoIiLlabcdefghjkmnpqrstvwxyz", "001111ABCDEFGHJKMNPQRSTVWXYZ"));
      Base62 = new BaseConverter("Base62", new Alphabet("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"));
    }

    // We use object pooling in the following way, if the pool is empty
    // a new object is created, this is effectively just as expensive as
    // creating a new string builder, if we succeed with our operation
    // we return (possibly a new string builder) to the pool. 
    // IT'S IMPORTANT THAT WE RESET THE STRING BUILDER BEFORE PUTTING IT BACK IN THE POOL.
    // If an exception occurs between the getting and putting back we simply
    // allow the object to get lost in the ether. It will eventually get garbage collected.

    internal static string Encode(this BaseConverter converter, byte v)
    {
      var sb = ConcurrentObjectPool<StringBuilder>.Get();
      converter.Encode(v, sb);
      var s = sb.ToString();
      // reset
      sb.Length = 0;
      ConcurrentObjectPool<StringBuilder>.Put(sb);
      return s;
    }

    internal static string Encode(this BaseConverter converter, ushort v)
    {
      var sb = ConcurrentObjectPool<StringBuilder>.Get();
      converter.Encode(v, sb);
      var s = sb.ToString();
      // reset
      sb.Length = 0;
      ConcurrentObjectPool<StringBuilder>.Put(sb);
      return s;
    }

    public static string Encode(this BaseConverter converter, int v)
    {
      var sb = ConcurrentObjectPool<StringBuilder>.Get();
      converter.Encode(v, sb);
      var s = sb.ToString();
      // reset
      sb.Length = 0;
      ConcurrentObjectPool<StringBuilder>.Put(sb);
      return s;
    }

    public static string Encode(this BaseConverter converter, uint v)
    {
      var sb = ConcurrentObjectPool<StringBuilder>.Get();
      converter.Encode(v, sb);
      var s = sb.ToString();
      // reset
      sb.Length = 0;
      ConcurrentObjectPool<StringBuilder>.Put(sb);
      return s;
    }

    public static string Encode(this BaseConverter converter, long v)
    {
      var sb = ConcurrentObjectPool<StringBuilder>.Get();
      converter.Encode(v, sb);
      var s = sb.ToString();
      // reset
      sb.Length = 0;
      ConcurrentObjectPool<StringBuilder>.Put(sb);
      return s;
    }

    public static string Encode(this BaseConverter converter, ulong v)
    {
      var sb = ConcurrentObjectPool<StringBuilder>.Get();
      converter.Encode(v, sb);
      var s = sb.ToString();
      // reset
      sb.Length = 0;
      ConcurrentObjectPool<StringBuilder>.Put(sb);
      return s;
    }

    public static string Encode(this BaseConverter converter, byte[] bytes)
    {
      return Encode(converter, bytes, 0, bytes.Length);
    }

    public static string Encode(this BaseConverter converter, byte[] bytes, int offset, int length)
    {
      var sb = ConcurrentObjectPool<StringBuilder>.Get();
      converter.Encode(bytes, offset, length, sb);
      var s = sb.ToString();
      // reset
      sb.Length = 0;
      ConcurrentObjectPool<StringBuilder>.Put(sb);
      return s;
    }

    public static string EncodeRemainder(this BaseConverter converter, byte[] bytes)
    {
      return EncodeRemainder(converter, bytes, 0, bytes.Length);
    }

    public static string EncodeRemainder(this BaseConverter converter, byte[] bytes, int offset, int length)
    {
      var sb = ConcurrentObjectPool<StringBuilder>.Get();
      converter.EncodeRemainder(bytes, offset, length, sb);
      var s = sb.ToString();
      // reset
      sb.Length = 0;
      ConcurrentObjectPool<StringBuilder>.Put(sb);
      return s;
    }

    public static byte[] DecodeBytes(this BaseConverter converter, string s)
    {
      return DecodeBytes(converter, s, 0, s.Length);
    }

    public static byte[] DecodeBytes(this BaseConverter converter, string s, int startIndex, int length)
    {
      var buffer = ConcurrentObjectPool<MemoryStream>.Get();
      converter.DecodeBytes(s, startIndex, length, buffer);
      var bytes = new byte[buffer.Length];
      buffer.Position = 0;
      buffer.Read(bytes, 0, bytes.Length);
      // reset
      buffer.SetLength(0);
      ConcurrentObjectPool<MemoryStream>.Put(buffer);
      return bytes;
    }

    public static byte[] DecodeRemainder(this BaseConverter converter, string s, int startIndex = 0)
    {
      var buffer = ConcurrentObjectPool<MemoryStream>.Get();
      var writer = new BinaryWriter(buffer);
      converter.DecodeRemainder(s, startIndex, writer);
      var bytes = new byte[buffer.Length];
      buffer.Position = 0;
      buffer.Read(bytes, 0, bytes.Length);
      // reset
      buffer.SetLength(0);
      ConcurrentObjectPool<MemoryStream>.Put(buffer);
      return bytes;
    }
  }
}
