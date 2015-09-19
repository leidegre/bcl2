using System;

namespace Bcl2.PositionalNotation
{
  public sealed class Alphabet
  {
    private readonly sbyte[] _decodeTable;
    private readonly char[] _encodeTable;

    public int Base { get { return _encodeTable.Length; } }

    // this class supports coding of various numerical bases 
    // (standard and non-standard) up to base 127

    private static char[] ToCharArray(string t)
    {
      return t != null ? t.ToCharArray() : null;
    }

    private void Translate(char[] f, char[] t)
    {
      // The translations can be used to map lower case to upper case or vice versa.
      // We don't expose this API since we don't want mutable alphabets.
      for (int i = 0; i < f.Length; i++)
      {
        var a = f[i];
        var b = t[i];
        if (_decodeTable[(int)a] == -1)
        {
          _decodeTable[(int)a] = (sbyte)Decode(b); // translate from a to b
        }
        else
        {
          throw new ArgumentException("Redefinition of character 0x" + (((int)a).ToString("X2")) + " '" + a + "' in translation table.");
        }
      }
    }

    public Alphabet(string s, string f = null, string t = null)
      : this(ToCharArray(s))
    {
      if ((f != null) ^ (t != null))
      {
        throw new ArgumentException("Either translation tables must to be set or both must be null.");
      }
      if ((f != null) & (t != null))
      {
        if (f.Length != t.Length)
        {
          throw new ArgumentException("Translation tables must be of same length.");
        }
        Translate(f.ToCharArray(), t.ToCharArray());
      }
    }

    private Alphabet(char[] s)
    {
      if (s == null)
      {
        throw new ArgumentNullException("s");
      }
      var r = new sbyte[128];
      for (int i = 0; i < 128; i++)
      {
        r[i] = -1;
      }
      for (int i = 0; i < s.Length; i++)
      {
        r[(int)s[i]] = (sbyte)i;
      }
      _encodeTable = s;
      _decodeTable = r;
    }

    public char Encode(int v)
    {
      var c = _encodeTable[v];
      return c;
    }

    public int Decode(char c)
    {
      var v = _decodeTable[c];
      if (v < 0)
      {
        throw new ArgumentOutOfRangeException("c", "The character 0x" + (((int)c).ToString("X2")) + " '" + c + "' is not valid for this base.");
      }
      return v;
    }

    public override string ToString()
    {
      return new string(_encodeTable);
    }
  }
}
