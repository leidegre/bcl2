using System;

namespace Bcl2.PositionalNotation
{
  public sealed class Alphabet
  {
    private readonly sbyte[] _decodeTable;
    private readonly char[] _encodeTable;
    private readonly bool _isLexicographic;
    private readonly bool _isCaseSensitive;

    public int Length { get { return _encodeTable.Length; } }

    /// <summary>
    /// True; if ordinal sort rules preserve lexicographical order.
    /// </summary>
    public bool IsLexicographic { get { return _isLexicographic; } }

    /// <summary>
    /// True; if alphabet is case sensitive. False; whenever there is a translation from upper case to corresponding lower case letter or vice versa.
    /// </summary>
    public bool IsCaseSensitive { get { return _isCaseSensitive; } }

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
      _isLexicographic = CheckLexicographic(s);
      _isCaseSensitive = CheckCaseSensitivity(s);
    }

    private static bool CheckLexicographic(string s)
    {
      for (int i = 1; i < s.Length; i++)
      {
        if (!(string.CompareOrdinal(s, i - 1, s, i, 1) < 0))
        {
          return false;
        }
      }
      return true;
    }

    private bool CheckCaseSensitivity(string s)
    {
      for (int i = 0; i < s.Length; i++)
      {
        var ch = s[i];
        int x = Decode(ch);
        if (char.IsLower(ch))
        {
          var upperInvariant = char.ToUpperInvariant(ch);
          int y;
          if (!(TryDecode(upperInvariant, out y) && x == y))
          {
            // case sensitive
            return true;
          }
        }
        if (char.IsUpper(ch))
        {
          var lowerInvariant = char.ToLowerInvariant(ch);
          int y;
          if (!(TryDecode(lowerInvariant, out y) && x == y))
          {
            // case sensitive
            return true;
          }
        }
      }
      return false;
    }

    public char Encode(int v)
    {
      var c = _encodeTable[v];
      return c;
    }

    public bool TryDecode(char c, out int v)
    {
      v = _decodeTable[c];
      return !(v < 0);
    }

    public int Decode(char c)
    {
      int v;
      if (!TryDecode(c, out v))
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
