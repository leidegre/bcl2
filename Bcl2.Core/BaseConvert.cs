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
    /// Base 16. Case insensitive.
    /// </summary>
    public static BaseConverter Hexadecimal { get; private set; }

    /// <summary>
    /// Base 32. Case insensitive. Triacontakaidecimal just one of many designs for base 32. This extends the hexadecimal base in a natural way using the characters 0123456789ABCDEFGHIJKLMNOPQRSTUV.
    /// </summary>
    public static BaseConverter Lexicographic32 { get; private set; }

    /// <summary>
    /// Base 32. Case insensitive. Crockford's base 32. For expressing numbers in a form that can be conveniently and accurately transmitted between humans and computer systems.
    /// </summary>
    /// <remarks>
    /// See http://www.crockford.com/wrmg/base32.html
    /// </remarks>
    public static BaseConverter Crockford { get; private set; }

    /// <summary>
    /// Base 32. Case insensitive. As per https://tools.ietf.org/html/rfc4648#section-4.
    /// </summary>
    public static BaseConverter Base32 { get; private set; }

    public static BaseConverter Base32Hex { get; private set; }

    /// <summary>
    /// Base 64. Case sensitive. As per https://tools.ietf.org/html/rfc4648#section-4.
    /// </summary>
    public static BaseConverter Base64 { get; private set; }

    public static BaseConverter Lexicographic64 { get; private set; }

    static BaseConvert()
    {
      Binary = new BaseConverter("Binary", new BitGroupCoding(2, Alphabets.Binary, paddingChar: '\0'));
      Decimal = new BaseConverter("Decimal", new BigIntCoding(10, Alphabets.Decimal));
      Hexadecimal = new BaseConverter("Hexadecimal", new BitGroupCoding(16, Alphabets.Hexadecimal, paddingChar: '\0'));
      Lexicographic32 = new BaseConverter("Lexicographic32", new BitGroupCoding(32, Alphabets.Triacontakaidecimal, paddingChar: '-'));
      Crockford = new BaseConverter("Crockford", new BitGroupCoding(32, Alphabets.Crockford, paddingChar: '-'));
      Base32 = new BaseConverter("Base32", new BitGroupCoding(32, Alphabets.Base32, paddingChar: '='));
      Base32Hex = new BaseConverter("Base32Hex", new BitGroupCoding(32, Alphabets.Triacontakaidecimal, paddingChar: '='));
      Base64 = new BaseConverter("Base64", new BitGroupCoding(64, Alphabets.Base64, paddingChar: '='));
      Lexicographic64 = new BaseConverter("Lexicographic64", new BitGroupCoding(64, Alphabets.Lexicographic, paddingChar: '-'));
    }

    // We use object pooling in the following way, if the pool is empty
    // a new object is created, this is effectively just as expensive as
    // creating a new string builder, if we succeed with our operation
    // we return (possibly a new string builder) to the pool. 
    // IT'S IMPORTANT THAT WE RESET THE STRING BUILDER BEFORE PUTTING IT BACK IN THE POOL.
    // If an exception occurs between the getting and putting back we simply
    // allow the object to get lost in the ether. It will eventually get garbage collected.

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
  }
}
