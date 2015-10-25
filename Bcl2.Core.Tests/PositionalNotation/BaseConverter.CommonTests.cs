using NUnit.Framework;
using System.Text;

namespace Bcl2.PositionalNotation
{
  partial class BaseConverterTests
  {
    [Test]
    public void BaseConverter_CommonDecimalTest()
    {
      var converter = BaseConvert.Decimal;

      var a = new[] {
        TestVector.Create(int.MinValue, "0000000000"),
        TestVector.Create(-1, "2147483647"),
        TestVector.Create(0, "2147483648"),
        TestVector.Create(+1, "2147483649"),
        TestVector.Create(int.MaxValue, "4294967295"),
      };

      var b = new[] {
        TestVector.Create(0U, "0000000000"),
        TestVector.Create(1U, "0000000001"),
        TestVector.Create(uint.MaxValue/2 + 1, "2147483648"),
        TestVector.Create(uint.MaxValue, "4294967295"),
      };

      var c = new[] {
        TestVector.Create(long.MinValue, "00000000000000000000"),
        TestVector.Create(-1L, "09223372036854775807"),
        TestVector.Create(0L, "09223372036854775808"),
        TestVector.Create(+1L, "09223372036854775809"),
        TestVector.Create(long.MaxValue, "18446744073709551615"),
      };

      var d = new[] {
        TestVector.Create(0UL, "00000000000000000000"),
        TestVector.Create(1UL, "00000000000000000001"),
        TestVector.Create((ulong)uint.MaxValue + 1, "00000000004294967296"),
        TestVector.Create(ulong.MaxValue, "18446744073709551615"),
      };

      var q = new[] {
        TestBuilder
          .NewTestVectorTest(a, converter, converter.Encode)
          .Create(),
        TestBuilder
          .NewTestVectorTest(b, converter, converter.Encode)
          .Create(),
        TestBuilder
          .NewTestVectorTest(c, converter, converter.Encode)
          .Create(),
        TestBuilder
          .NewTestVectorTest(d, converter, converter.Encode)
          .Create(),
      };

      foreach (var test in q)
      {
        test();
      }
    }

    [Test]
    public void BaseConverter_CommonBase64Test()
    {
      var converter = BaseConvert.Base64;

      // See https://tools.ietf.org/html/rfc4648#page-12

      var a = new[] {
        TestVector.Create(Encoding.ASCII.GetBytes(""), ""),
        TestVector.Create(Encoding.ASCII.GetBytes("f"), "Zg=="),
        TestVector.Create(Encoding.ASCII.GetBytes("fo"), "Zm8="),
        TestVector.Create(Encoding.ASCII.GetBytes("foo"), "Zm9v"),
        TestVector.Create(Encoding.ASCII.GetBytes("foob"), "Zm9vYg=="),
        TestVector.Create(Encoding.ASCII.GetBytes("fooba"), "Zm9vYmE="),
        TestVector.Create(Encoding.ASCII.GetBytes("foobar"), "Zm9vYmFy"),
      };

      var q = new[] {
        TestBuilder
          .NewTestVectorTest(a, converter, converter.Encode)
          .Create(),
      };

      foreach (var test in q)
      {
        test();
      }
    }

    [Test]
    public void BaseConverter_CommonBase32Test()
    {
      var converter = BaseConvert.Base32;

      // See https://tools.ietf.org/html/rfc4648#page-12

      var a = new[] {
        TestVector.Create(Encoding.ASCII.GetBytes(""), ""),
        TestVector.Create(Encoding.ASCII.GetBytes("f"), "MY======"),
        TestVector.Create(Encoding.ASCII.GetBytes("fo"), "MZXQ===="),
        TestVector.Create(Encoding.ASCII.GetBytes("foo"), "MZXW6==="),
        TestVector.Create(Encoding.ASCII.GetBytes("foob"), "MZXW6YQ="),
        TestVector.Create(Encoding.ASCII.GetBytes("fooba"), "MZXW6YTB"),
        TestVector.Create(Encoding.ASCII.GetBytes("foobar"), "MZXW6YTBOI======"),
      };

      var q = new[] {
        TestBuilder
          .NewTestVectorTest(a, converter, converter.Encode)
          .Create(),
      };

      foreach (var test in q)
      {
        test();
      }
    }

    [Test]
    public void BaseConverter_CommonBase32HexTest()
    {
      var converter = BaseConvert.Base32Hex;

      // See https://tools.ietf.org/html/rfc4648#page-12

      var a = new[] {
        TestVector.Create(Encoding.ASCII.GetBytes(""), ""),
        TestVector.Create(Encoding.ASCII.GetBytes("f"), "CO======"),
        TestVector.Create(Encoding.ASCII.GetBytes("fo"), "CPNG===="),
        TestVector.Create(Encoding.ASCII.GetBytes("foo"), "CPNMU==="),
        TestVector.Create(Encoding.ASCII.GetBytes("foob"), "CPNMUOG="),
        TestVector.Create(Encoding.ASCII.GetBytes("fooba"), "CPNMUOJ1"),
        TestVector.Create(Encoding.ASCII.GetBytes("foobar"), "CPNMUOJ1E8======"),
      };

      var q = new[] {
        TestBuilder
          .NewTestVectorTest(a, converter, converter.Encode)
          .Create(),
      };

      foreach (var test in q)
      {
        test();
      }
    }

    [Test]
    public void BaseConverter_CommonHexadecimalTest()
    {
      var converter = BaseConvert.Hexadecimal;

      // See https://tools.ietf.org/html/rfc4648#page-12

      var a = new[] {
        TestVector.Create(Encoding.ASCII.GetBytes(""), ""),
        TestVector.Create(Encoding.ASCII.GetBytes("f"), "66"),
        TestVector.Create(Encoding.ASCII.GetBytes("fo"), "666F"),
        TestVector.Create(Encoding.ASCII.GetBytes("foo"), "666F6F"),
        TestVector.Create(Encoding.ASCII.GetBytes("foob"), "666F6F62"),
        TestVector.Create(Encoding.ASCII.GetBytes("fooba"), "666F6F6261"),
        TestVector.Create(Encoding.ASCII.GetBytes("foobar"), "666F6F626172"),
      };

      var q = new[] {
        TestBuilder
          .NewTestVectorTest(a, converter, converter.Encode)
          .Create(),
      };

      foreach (var test in q)
      {
        test();
      }
    }
  }
}
