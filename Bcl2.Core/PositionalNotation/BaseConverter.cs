using System;
using System.IO;
using System.Text;

namespace Bcl2.PositionalNotation
{
  public class BaseConverter
  {
    private readonly string _name;
    public string Name { get { return _name; } }
    private readonly Alphabet _alphabet;
    public int Base { get { return _alphabet.Base; } }
    private readonly int _charCountPerByte;
    private readonly int _charCountPerUInt16;
    private readonly int _charCountPerUInt32;
    private readonly int _charCountPerUInt64;

    /// <remarks>
    /// There's a subtle coding issue with some bases where the char count required for 7 bytes is
    /// indistinguishable from 8 bytes. To resolve this ambiguity, we use a single padding char.
    /// This padding is only applicable to the coding of byte strings and only when the length is evenly divisible by 7.
    /// </remarks>
    private readonly bool _hasCodingIssue;

    public BaseConverter(string name, Alphabet alphabet)
    {
      _name = name;
      _alphabet = alphabet;
      _charCountPerByte = (int)Math.Ceiling(Math.Log(Math.Pow(2, 8), alphabet.Base));
      _charCountPerUInt16 = (int)Math.Ceiling(Math.Log(Math.Pow(2, 16), alphabet.Base));
      _charCountPerUInt32 = (int)Math.Ceiling(Math.Log(Math.Pow(2, 32), alphabet.Base));
      _charCountPerUInt64 = (int)Math.Ceiling(Math.Log(Math.Pow(2, 64), alphabet.Base));
      if ((_charCountPerByte + _charCountPerUInt16 + _charCountPerUInt32) == _charCountPerUInt64)
      {
        _hasCodingIssue = true;
      }
    }

    //// Encode

    // coding is private because we only use it for binary strings
    internal void Encode(byte value, StringBuilder sb)
    {
      Encode(value, _alphabet, _charCountPerByte, sb);
    }

    // coding is private because we only use it for binary strings
    internal void Encode(ushort value, StringBuilder sb)
    {
      Encode(value, _alphabet, _charCountPerUInt16, sb);
    }

    public void Encode(int value, StringBuilder sb)
    {
      // This will maintain that negative numbers 
      // are still lexicographically ordered after encoding.
      var v = int.MinValue ^ value;
      Encode((uint)v, sb);
    }

    public void Encode(uint value, StringBuilder sb)
    {
      Encode(value, _alphabet, _charCountPerUInt32, sb);
    }

    public void Encode(long value, StringBuilder sb)
    {
      // This will maintain that negative numbers 
      // are still lexicographically ordered after encoding.
      var v = long.MinValue ^ value;
      Encode((ulong)v, sb);
    }

    public void Encode(ulong value, StringBuilder sb)
    {
      Encode(value, _alphabet, _charCountPerUInt64, sb);
    }

    private static void Encode(ulong value, Alphabet a, int charCountPerWord, StringBuilder sb)
    {
      var b = (ulong)a.Base;
      sb.Append(' ', charCountPerWord);
      var startIndex = sb.Length - 1;
      for (int i = 0; i < charCountPerWord; i++)
      {
        var x = value % b;
        value /= b;
        var c = a.Encode((int)x);
        sb[startIndex - i] = c;
      }
    }

    public void Encode(byte[] bytes, int offset, int length, StringBuilder sb)
    {
      while (length >= 8)
      {
        var v = BitConverter.ToUInt64(bytes, offset);
        if (BitConverter.IsLittleEndian)
        {
          // for correctness yo! (trivia: Xbox360 is big endian and it can run .NET yay!)
          v = v.ByteSwap();
        }
        Encode(v, sb);
        offset += 8;
        length -= 8;
      }
      bool appendPaddingChar = _hasCodingIssue & (length == 7);
#if DELIMITER
      sb.Append('-', 1);
#endif
      if (length >= 4)
      {
        var v = BitConverter.ToUInt32(bytes, offset);
        if (BitConverter.IsLittleEndian)
        {
          v = v.ByteSwap();
        }
        Encode(v, sb);
        offset += 4;
        length -= 4;
      }
#if DELIMITER
      sb.Append('-', 1);
#endif
      if (length >= 2)
      {
        var v = BitConverter.ToUInt16(bytes, offset);
        if (BitConverter.IsLittleEndian)
        {
          v = v.ByteSwap();
        }
        Encode(v, sb);
        offset += 2;
        length -= 2;
      }
#if DELIMITER
      sb.Append('-', 1);
#endif
      if (length == 1)
      {
        var v = bytes[offset];
        Encode(v, sb);
        offset += 1;
        length -= 1;
      }
      if (appendPaddingChar)
      {
        sb.Append(_alphabet.Encode(0)); // padding char
      }
    }

    //// Decode

    // coding is private because we only use it for binary strings
    internal byte DecodeByte(string s, int startIndex = 0)
    {
      return (byte)Decode(s, startIndex, _alphabet, _charCountPerByte);
    }

    // coding is private because we only use it for binary strings
    internal ushort DecodeUInt16(string s, int startIndex = 0)
    {
      return (ushort)Decode(s, startIndex, _alphabet, _charCountPerUInt16);
    }

    public int DecodeInt32(string s, int startIndex = 0)
    {
      uint v = DecodeUInt32(s, startIndex);
      return (int)v ^ int.MinValue;
    }

    public uint DecodeUInt32(string s, int startIndex = 0)
    {
      return (uint)Decode(s, startIndex, _alphabet, _charCountPerUInt32);
    }

    public long DecodeInt64(string s, int startIndex = 0)
    {
      ulong v = DecodeUInt64(s, startIndex);
      return (long)v ^ long.MinValue;
    }

    public ulong DecodeUInt64(string s, int startIndex = 0)
    {
      return Decode(s, startIndex, _alphabet, _charCountPerUInt64);
    }

    private static ulong Decode(string s, int startIndex, Alphabet a, int charCountPerWord)
    {
      ulong v = 0;
      ulong b = (ulong)a.Base;
      for (int i = 0; i < charCountPerWord; i++)
      {
        var x = (ulong)a.Decode(s[startIndex + i]);
        v = b * v + x;
      }
      return v;
    }

    public void DecodeBytes(string s, int startIndex, int length, Stream outputStream)
    {
#if DELIMITER
      length -= 3;
#endif
      var writer = new BinaryWriter(outputStream);
      bool removePaddingChar = false;
      if (_hasCodingIssue)
      {
        var summationFixCharCount = _charCountPerUInt64 + 1;
        while ((length >= _charCountPerUInt64) & (length != summationFixCharCount))
        {
          var v = DecodeUInt64(s, startIndex);
          if (BitConverter.IsLittleEndian)
          {
            v = v.ByteSwap();
          }
          writer.Write(v);
          startIndex += _charCountPerUInt64;
          length -= _charCountPerUInt64;
        }
        removePaddingChar = (length == summationFixCharCount);
      }
      else
      {
        while (length >= _charCountPerUInt64)
        {
          var v = DecodeUInt64(s, startIndex);
          if (BitConverter.IsLittleEndian)
          {
            v = v.ByteSwap();
          }
          writer.Write(v);
          startIndex += _charCountPerUInt64;
          length -= _charCountPerUInt64;
        }
      }
#if DELIMITER
      startIndex += 1;
#endif
      if (length >= _charCountPerUInt32)
      {
        var v = DecodeUInt32(s, startIndex);
        if (BitConverter.IsLittleEndian)
        {
          v = v.ByteSwap();
        }
        writer.Write(v);
        startIndex += _charCountPerUInt32;
        length -= _charCountPerUInt32;
      }
#if DELIMITER
      startIndex += 1;
#endif
      if (length >= _charCountPerUInt16)
      {
        var v = DecodeUInt16(s, startIndex);
        if (BitConverter.IsLittleEndian)
        {
          v = v.ByteSwap();
        }
        writer.Write(v);
        startIndex += _charCountPerUInt16;
        length -= _charCountPerUInt16;
      }
#if DELIMITER
      startIndex += 1;
#endif
      if (length >= _charCountPerByte)
      {
        var v = DecodeByte(s, startIndex);
        writer.Write(v);
        startIndex += _charCountPerByte;
        length -= _charCountPerByte;
      }
      if (removePaddingChar)
      {
        startIndex += 1;
        length -= 1;
      }
    }

    //// 

    public override string ToString()
    {
      return Name;
    }
  }
}
