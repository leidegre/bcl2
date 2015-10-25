using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bcl2.PositionalNotation
{
  public static class Alphabets
  {
    public static Alphabet Binary { get; private set; }
    public static Alphabet Decimal { get; private set; }
    public static Alphabet Hexadecimal { get; private set; }
    public static Alphabet Triacontakaidecimal { get; private set; }
    public static Alphabet Crockford { get; private set; }
    public static Alphabet Base32 { get; private set; }
    public static Alphabet Base64 { get; private set; }
    public static Alphabet Lexicographic { get; private set; }

    static Alphabets()
    {
      Binary = new Alphabet("01");
      Decimal = new Alphabet("0123456789");
      Hexadecimal = new Alphabet("0123456789ABCDEF"
        , "abcdef"
        , "ABCDEF"
        );
      Triacontakaidecimal = new Alphabet("0123456789ABCDEFGHIJKLMNOPQRSTUV" // aka Lexicographic32
        , "abcdefghijklmnopqrstuv"
        , "ABCDEFGHIJKLMNOPQRSTUV"
        );
      Crockford = new Alphabet("0123456789ABCDEFGHJKMNPQRSTVWXYZ"
        , "OoIiLlabcdefghjkmnpqrstvwxyz"
        , "001111ABCDEFGHJKMNPQRSTVWXYZ"
        );
      Base32 = new Alphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZ234567"
        , "abcdefghijklmnopqrstuvwxyz"
        , "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        );
      Base64 = new Alphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/");
      Lexicographic = new Alphabet(".0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz"); // -.0-9A-Z_a-z~
    }
  }
}
