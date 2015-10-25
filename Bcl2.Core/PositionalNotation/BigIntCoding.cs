using System;
using System.IO;
using System.Text;

namespace Bcl2.PositionalNotation
{
  public class BigIntCoding : BaseCoding
  {
    private readonly uint _numBase;
    private readonly double _log2;
    private readonly double _logBase;

    public override bool IsLexicographic { get { return false; } }

    public BigIntCoding(int numBase, Alphabet abc)
      : base(numBase, abc)
    {
      this._numBase = (uint)numBase;
      this._log2 = Math.Log(2);
      this._logBase = Math.Log(numBase);
    }

    private int GetCharCount(int byteCount)
    {
      var charCount = (int)Math.Ceiling((8d * byteCount * _log2) / _logBase);
      return charCount;
    }

    public override void Encode(byte[] bytes, int offset, int count, StringBuilder sb)
    {
      var bigInt = new BigInt(bytes, offset, count);
      var abc = Alphabet;
      int charCount = GetCharCount(count);
      sb.Length += charCount; // reserve capacity
      var startIndex = sb.Length;
      for (int i = 0; i < charCount; i++)
      {
        var x = (int)bigInt.DivRem(_numBase);
        var ch = abc.Encode(x);
        sb[--startIndex] = ch;
      }
    }

    private int GetByteCount(int length)
    {
      var byteCount = (int)Math.Floor((length * _logBase) / (8 * _log2));
      return byteCount;
    }

    public override void DecodeBytes(string s, int startIndex, int length, Stream outputStream)
    {
      var abc = Alphabet;

      int byteCount = GetByteCount(length);

      var bigInt = BigInt.FromByteCount(byteCount);

      for (int i = startIndex, end = startIndex + length; i < end; i++)
      {
        var ch = s[i];
        var x = (uint)abc.Decode(ch);
        bigInt.Mul(_numBase);
        bigInt.Add(x);
      }

      var bytes = bigInt.ToByteArray();
      outputStream.Write(bytes, 0, byteCount);
    }
  }
}
