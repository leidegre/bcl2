using System;
using System.IO;
using System.Text;

namespace Bcl2.PositionalNotation
{
  public abstract class BaseCoding
  {
    public int Base { get; private set; }
    private readonly Alphabet _abc;
    public Alphabet Alphabet { get { return _abc; } }
    public abstract bool IsLexicographic { get; }

    protected BaseCoding(int numBase, Alphabet abc)
    {
      if (!((2 <= numBase) & (numBase <= 128)))
      {
        throw new ArgumentOutOfRangeException("numBase", "The numerical base has to be between 2 and 128.");
      }
      if (abc == null)
      {
        throw new ArgumentNullException("abc");
      }
      if (!(Base <= abc.Length))
      {
        throw new InvalidOperationException("The numerical base is out of range of the alphabet.");
      }
      Base = numBase;
      _abc = abc;
    }

    public virtual void Encode(uint v, StringBuilder sb)
    {
      // by default, we just treat a 32-bit group as a sequence of bytes.
      var bytes = BitConverter.GetBytes(v.ByteSwap());
      Encode(bytes, 0, bytes.Length, sb);
    }

    public virtual void Encode(ulong v, StringBuilder sb)
    {
      // by default, we just treat a 64-bit group as a sequence of bytes.
      var bytes = BitConverter.GetBytes(v.ByteSwap());
      Encode(bytes, 0, bytes.Length, sb);
    }

    public abstract void Encode(byte[] bytes, int offset, int count, StringBuilder sb);

    public virtual uint DecodeUInt32(string s, int startIndex, int length)
    {
      var temp = new byte[4];
      DecodeBytes(s, startIndex, s.Length - startIndex, new MemoryStream(temp, 0, 4, true));
      return BigEndian.ToUInt32(temp, 0);
    }

    public virtual ulong DecodeUInt64(string s, int startIndex, int length)
    {
      var temp = new byte[8];
      DecodeBytes(s, startIndex, s.Length - startIndex, new MemoryStream(temp, 0, 8, true));
      return BigEndian.ToUInt64(temp, 0);
    }

    public abstract void DecodeBytes(string s, int startIndex, int length, Stream outputStream);
  }
}
