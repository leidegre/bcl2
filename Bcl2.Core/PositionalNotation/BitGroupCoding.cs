using System;
using System.IO;
using System.Text;

namespace Bcl2.PositionalNotation
{
  public class BitGroupCoding : BaseCoding
  {
    private readonly int _bitCount;
    private readonly ulong _bitMask;
    private readonly int _bitGroupByteCount;
    private readonly int _bitGroupCharCount;
    private readonly int[] _charCountFromByteCount;
    private readonly int[] _byteCountFromCharCount;
    private readonly char _paddingChar;

    public override bool IsLexicographic { get { return _paddingChar < Alphabet.Encode(0); } }

    public BitGroupCoding(int numBase, Alphabet abc, char paddingChar)
      : base(numBase, abc)
    {
      if (!BitTwiddling.IsPowerOfTwo(numBase))
      {
        throw new ArgumentOutOfRangeException("numBase", "The '" + typeof(BitGroupCoding) + "' can only be used with a numerical base that is a power of 2.");
      }
      int v;
      if (abc.TryDecode(_paddingChar, out v))
      {
        throw new ArgumentOutOfRangeException("The padding char cannot be a member of the alphabet.");
      }

      // Hmm...
      var bitCount = (int)Math.Log(numBase, 2);
      _bitCount = bitCount;
      _bitMask = (1u << bitCount) - 1;

      var bitGroupSize = BitTwiddling.LeastCommonMultiple(bitCount, 8);
      var bitGroupByteCount = bitGroupSize / 8;
      var bitGroupCharCount = bitGroupSize / bitCount;

      // Encode
      var charCountFromByteCount = new int[1 + bitGroupByteCount];
      for (int i = 0; i <= bitGroupByteCount; i++)
      {
        charCountFromByteCount[i] = (8 * i + (bitCount - 1)) / bitCount;
      }

      // Decode
      var byteCountFromCharCount = new int[1 + bitGroupCharCount];
      for (int i = 0; i < byteCountFromCharCount.Length; i++)
      {
        byteCountFromCharCount[i] = -1;
      }
      for (int i = 1; i <= bitGroupByteCount; i++)
      {
        byteCountFromCharCount[charCountFromByteCount[i]] = i;
      }

      // Save
      _bitGroupByteCount = bitGroupByteCount;
      _bitGroupCharCount = bitGroupCharCount;
      _charCountFromByteCount = charCountFromByteCount;
      _byteCountFromCharCount = byteCountFromCharCount;
      _paddingChar = paddingChar;
    }

    public override void Encode(byte[] bytes, int offset, int count, StringBuilder sb)
    {
      var abc = Alphabet;
      while (count >= _bitGroupByteCount)
      {
        // load bit group
        ulong v = 0;
        for (int i = 0; i < _bitGroupByteCount; i++)
        {
          var bytePosition = (_bitGroupByteCount - (i + 1)) << 3;
          var mask = (ulong)bytes[offset + i] << bytePosition;
          v |= mask;
        }
        // encode bit group
        for (int i = 0; i < _bitGroupCharCount; i++)
        {
          var bitPosition = (_bitGroupCharCount - (i + 1)) * _bitCount;
          int x = (int)((v >> bitPosition) & _bitMask);
          var ch = abc.Encode(x);
          sb.Append(ch);
        }
        offset += _bitGroupByteCount;
        count -= _bitGroupByteCount;
      }
      // padding?
      if (count > 0)
      {
        // load bit group
        ulong v = 0;
        for (int i = 0; i < count; i++)
        {
          var bytePosition = (_bitGroupByteCount - (i + 1)) << 3;
          var mask = (ulong)bytes[offset + i] << bytePosition;
          v |= mask;
        }
        // the rest is just zero

        var charCount = _charCountFromByteCount[count];

        // encode bit group
        for (int i = 0; i < charCount; i++)
        {
          var bitPosition = (_bitGroupCharCount - (i + 1)) * _bitCount;
          int x = (int)((v >> bitPosition) & _bitMask);
          var ch = abc.Encode(x);
          sb.Append(ch);
        }

        var padding = _bitGroupCharCount - charCount;
        sb.Append(_paddingChar, padding);
      }
    }

    public override void DecodeBytes(string s, int startIndex, int length, Stream outputStream)
    {
      var abc = Alphabet;
      var temp = new byte[8];
      var bitGroup = new BitGroup(_bitCount);
      while (length > _bitGroupCharCount)
      {
        // this is the faster code path
        // it does not need to check for padding
        // or rather it does not expect to encounter any padding
        bitGroup.Reset();
        for (int i = 0; i < _bitGroupCharCount; i++)
        {
          var c = s[startIndex + i];
          var x = (uint)abc.Decode(c);
          bitGroup.Write(x);
        }
        bitGroup.Read(temp, 0, _bitGroupByteCount);
        outputStream.Write(temp, 0, _bitGroupByteCount);
        startIndex += _bitGroupCharCount;
        length -= _bitGroupCharCount;
      }
      // padding?
      if (length > 0)
      {
        bitGroup.Reset();
        int i, padding = 0;
        for (i = 0; i < _bitGroupCharCount; i++)
        {
          var c = s[startIndex + i];
          if (c == _paddingChar)
          {
            bitGroup.Write(0);
            padding++;
            continue;
          }
          var x = (uint)abc.Decode(c);
          bitGroup.Write(x);
        }
        bitGroup.Read(temp, 0, _bitGroupByteCount);
        var byteCount = _byteCountFromCharCount[i - padding];
        if (byteCount == -1)
        {
          throw new InvalidOperationException("Padding is invalid.");
        }
        outputStream.Write(temp, 0, byteCount);
      }
    }
  }
}
