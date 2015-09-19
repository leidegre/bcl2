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

    public BaseConverter(string name, Alphabet alphabet)
    {
      _name = name;
      _alphabet = alphabet;
      _charCountPerByte = (int)Math.Ceiling(Math.Log(Math.Pow(2, 8), alphabet.Base));
      _charCountPerUInt16 = (int)Math.Ceiling(Math.Log(Math.Pow(2, 16), alphabet.Base));
      _charCountPerUInt32 = (int)Math.Ceiling(Math.Log(Math.Pow(2, 32), alphabet.Base));
      _charCountPerUInt64 = (int)Math.Ceiling(Math.Log(Math.Pow(2, 64), alphabet.Base));
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
        var v = BigEndian.ToUInt64(bytes, offset);
        Encode(v, sb);
        offset += 8;
        length -= 8;
      }
      // remainder?
      if ((length & 7) != 0)
      {
        EncodeRemainder(bytes, offset, length, sb);
      }
    }

    public void EncodeRemainder(byte[] bytes, int offset, int length, StringBuilder sb)
    {
      ulong v = 0;
      switch (length & 7)
      {
        case 7:
          {
            v |= (ulong)BigEndian.ToUInt32(bytes, offset) << 32;
            v |= (ulong)BigEndian.ToUInt16(bytes, offset + 4) << 16;
            v |= (ulong)bytes[offset + 6] << 8;
            break;
          }
        case 6:
          {
            v |= (ulong)BigEndian.ToUInt32(bytes, offset) << 32;
            v |= (ulong)BigEndian.ToUInt16(bytes, offset + 4) << 16;
            break;
          }
        case 5:
          {
            v |= (ulong)BigEndian.ToUInt32(bytes, offset) << 32;
            v |= (ulong)bytes[offset + 4] << 24;
            break;
          }
        case 4:
          {
            v |= (ulong)BigEndian.ToUInt32(bytes, offset) << 32;
            break;
          }
        case 3:
          {
            v |= (ulong)BigEndian.ToUInt16(bytes, offset) << 48;
            v |= (ulong)bytes[offset + 2] << 40;
            break;
          }
        case 2:
          {
            v |= (ulong)BigEndian.ToUInt16(bytes, offset) << 48;
            break;
          }
        case 1:
          {
            v |= (ulong)bytes[offset] << 56;
            break;
          }
      }
      v |= ((uint)length & 7);
      Encode(v, sb);
      sb.Append(_alphabet.Encode(0)); // padding char
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
      var writer = new BinaryWriter(outputStream);
      var paddingCharLength = _charCountPerUInt64 + 1;
      while ((length >= _charCountPerUInt64) & (length != paddingCharLength))
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
      if (length == paddingCharLength)
      {
        DecodeRemainder(s, startIndex, writer);
      }
    }

    public void DecodeRemainder(string s, int startIndex, BinaryWriter writer)
    {
      var v = DecodeUInt64(s, startIndex);
      switch (v & 7)
      {
        case 7:
          {
            var a = (uint)(v >> 32);
            writer.Write(a.ByteSwap());
            var b = (ushort)(v >> 16);
            writer.Write(b.ByteSwap());
            writer.Write((byte)(v >> 8));
            break;
          }
        case 6:
          {
            var a = (uint)(v >> 32);
            writer.Write(a.ByteSwap());
            var b = (ushort)(v >> 16);
            writer.Write(b.ByteSwap());
            break;
          }
        case 5:
          {
            var a = (uint)(v >> 32);
            writer.Write(a.ByteSwap());
            writer.Write((byte)(v >> 24));
            break;
          }
        case 4:
          {
            var a = (uint)(v >> 32);
            writer.Write(a.ByteSwap());
            break;
          }
        case 3:
          {
            var b = (ushort)(v >> 48);
            writer.Write(b.ByteSwap());
            writer.Write((byte)(v >> 40));
            break;
          }
        case 2:
          {
            var b = (ushort)(v >> 48);
            writer.Write(b.ByteSwap());
            break;
          }
        case 1:
          {
            writer.Write((byte)(v >> 56));
            break;
          }
      }
    }

    //// 

    public override string ToString()
    {
      return Name;
    }
  }
}
