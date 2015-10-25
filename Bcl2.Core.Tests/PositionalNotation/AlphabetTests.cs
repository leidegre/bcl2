using NUnit.Framework;
using System;

namespace Bcl2.PositionalNotation
{
  [TestFixture]
  public class AlphabetTests
  {
    [Test]
    public void Alphabet_EncodeTest()
    {
      var hex = Alphabets.Hexadecimal;
      var s = "0123456789ABCDEF";
      for (int i = 0; i < 16; i++)
      {
        var ch = hex.Encode(i);
        Assert.AreEqual(s[i], ch);
      }
    }

    [Test]
    public void Alphabet_DecodeTest()
    {
      var hex = Alphabets.Hexadecimal;
      var s = "0123456789ABCDEF";
      for (int i = 0; i < 16; i++)
      {
        var x = hex.Decode(s[i]);
        Assert.AreEqual(i, x);
      }
    }

    [Test]
    public void Alphabet_DecodeIgnoreCaseTest()
    {
      var hex = Alphabets.Hexadecimal;
      var s = "0123456789abcdef";
      for (int i = 0; i < 16; i++)
      {
        var x = hex.Decode(s[i]);
        Assert.AreEqual(i, x);
      }
    }

    // note that the run-time does throw an IndexOutOfRange, not an ArgumentOutOfRange exception here
    [Test, ExpectedException(typeof(IndexOutOfRangeException))]
    public void Alphabet_EncodeOutOfRangeTest()
    {
      Alphabets.Hexadecimal.Encode(16);
    }

    [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Alphabet_DecodeOutOfRangeTest()
    {
      Alphabets.Hexadecimal.Decode('G');
    }

    [Test]
    public void Alphabet_TryDecode()
    {
      int x;
      Assert.IsFalse(Alphabets.Hexadecimal.TryDecode('G', out x));
      int y;
      Assert.IsTrue(Alphabets.Hexadecimal.TryDecode('F', out y));
    }

    [Test]
    public void Alphabet_IsLexicographicTest()
    {
      Assert.IsTrue(Alphabets.Binary.IsLexicographic);
      Assert.IsTrue(Alphabets.Decimal.IsLexicographic);
      Assert.IsTrue(Alphabets.Hexadecimal.IsLexicographic);
      Assert.IsTrue(Alphabets.Triacontakaidecimal.IsLexicographic);
      Assert.IsTrue(Alphabets.Crockford.IsLexicographic);
      Assert.IsFalse(Alphabets.Base32.IsLexicographic);
      Assert.IsFalse(Alphabets.Base64.IsLexicographic);
      Assert.IsTrue(Alphabets.Lexicographic.IsLexicographic);
    }

    [Test]
    public void Alphabet_IsCaseSensitiveTest()
    {
      Assert.IsFalse(Alphabets.Binary.IsCaseSensitive);
      Assert.IsFalse(Alphabets.Decimal.IsCaseSensitive);
      Assert.IsFalse(Alphabets.Hexadecimal.IsCaseSensitive);
      Assert.IsFalse(Alphabets.Triacontakaidecimal.IsCaseSensitive);
      Assert.IsFalse(Alphabets.Crockford.IsCaseSensitive);
      Assert.IsFalse(Alphabets.Base32.IsCaseSensitive);
      Assert.IsTrue(Alphabets.Base64.IsCaseSensitive);
      Assert.IsTrue(Alphabets.Lexicographic.IsCaseSensitive);
    }
  }
}
