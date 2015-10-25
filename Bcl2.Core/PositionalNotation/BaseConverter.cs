using System;
using System.IO;
using System.Text;

namespace Bcl2.PositionalNotation
{
  public class BaseConverter
  {
    private readonly string _name;
    public string Name { get { return _name ?? string.Empty; } }
    private readonly BaseCoding _coding;
    public BaseCoding Coding { get { return _coding; } }

    public BaseConverter(string name, BaseCoding coding)
    {
      if (coding == null)
      {
        throw new ArgumentNullException("coding");
      }
      _name = name;
      _coding = coding;
    }

    // These will maintain that negative numbers 
    // are still lexicographically ordered after encoding.

    public static uint EncodeLexicographic(int v)
    {
      var u = int.MinValue ^ v;
      return (uint)u;
    }

    public static int DecodeLexicographic(uint u)
    {
      var v = (int)u;
      return v ^ int.MinValue;
    }

    public static ulong EncodeLexicographic(long v)
    {
      var u = long.MinValue ^ v;
      return (ulong)u;
    }

    public static long DecodeLexicographic(ulong u)
    {
      var v = (long)u;
      return v ^ long.MinValue;
    }

    //// Encode

    public void Encode(int value, StringBuilder sb)
    {
      var u = EncodeLexicographic(value);
      Encode(u, sb);
    }

    public void Encode(uint value, StringBuilder sb)
    {
      _coding.Encode(value, sb);
    }

    public void Encode(long value, StringBuilder sb)
    {
      var u = EncodeLexicographic(value);
      Encode(u, sb);
    }

    public void Encode(ulong value, StringBuilder sb)
    {
      _coding.Encode(value, sb);
    }

    public void Encode(byte[] bytes, int offset, int length, StringBuilder sb)
    {
      _coding.Encode(bytes, offset, length, sb);
    }
    
    //// Decode

    public int DecodeInt32(string s)
    {
      if (s == null)
      {
        throw new ArgumentNullException("s");
      }
      return DecodeInt32(s, 0, s.Length);
    }

    public int DecodeInt32(string s, int startIndex, int length)
    {
      uint u = DecodeUInt32(s, startIndex, length);
      var v = DecodeLexicographic(u);
      return v;
    }

    public uint DecodeUInt32(string s)
    {
      if (s == null)
      {
        throw new ArgumentNullException("s");
      }
      var u = DecodeUInt32(s, 0, s.Length);
      return u;
    }

    public uint DecodeUInt32(string s, int startIndex, int length)
    {
      var u = _coding.DecodeUInt32(s, startIndex, length);
      return u;
    }

    public long DecodeInt64(string s)
    {
      if (s == null)
      {
        throw new ArgumentNullException("s");
      }
      var u = DecodeInt64(s, 0, s.Length);
      return u;
    }

    public long DecodeInt64(string s, int startIndex, int length)
    {
      ulong u = DecodeUInt64(s, startIndex, length);
      var v = DecodeLexicographic(u);
      return v;
    }

    public ulong DecodeUInt64(string s)
    {
      if (s == null)
      {
        throw new ArgumentNullException("s");
      }
      var u = _coding.DecodeUInt64(s, 0, s.Length);
      return u;
    }

    public ulong DecodeUInt64(string s, int startIndex, int length)
    {
      var u = _coding.DecodeUInt64(s, startIndex, length);
      return u;
    }

    public void DecodeBytes(string s, int startIndex, int length, Stream outputStream)
    {
      _coding.DecodeBytes(s, startIndex, length, outputStream);
    }

    //// 

    public override string ToString()
    {
      return Name;
    }
  }
}
