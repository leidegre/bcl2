using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bcl2.PositionalNotation
{
  partial class BaseConverterTests
  {
    public IEnumerable<BaseConverter> GetBaseTestData()
    {
      yield return BaseConvert.Binary;
      yield return BaseConvert.Decimal;
      yield return BaseConvert.Hexadecimal;
      yield return BaseConvert.Lexicographic32;
      yield return BaseConvert.Base32;
      yield return BaseConvert.Base32Hex;
      yield return BaseConvert.Crockford;
      yield return BaseConvert.Base64;
      yield return BaseConvert.Lexicographic64;
    }

    [Test, TestCaseSource("GetBaseTestData")]
    public void BaseConverter_BaseExpIntsWithSmallVarCodingTest(BaseConverter converter)
    {
      var q = Enumerable.Range(0, 64)
        .SelectMany(x => {
          var log = (ulong)Math.Pow(converter.Coding.Base, x);
          return new[] { log - 1, log, log + 1 };
        });

      var a = q.TakeWhile(x => x <= int.MaxValue).Select(x => (int)x);
      var b = q.TakeWhile(x => x <= uint.MaxValue).Select(x => (uint)x);
      var c = q.TakeWhile(x => x <= long.MaxValue).Select(x => (long)x);
      var d = q;

      var actions = new Action[] {
        TestBuilder
          .NewCodingTest(a, converter, converter.Encode, x => converter.DecodeInt32(x))
          .Create(),
        TestBuilder
          .NewCodingTest(b, converter, converter.Encode, x => converter.DecodeUInt32(x))
          .Create(),
        TestBuilder
          .NewCodingTest(c, converter, converter.Encode, x => converter.DecodeInt64(x))
          .Create(),
        TestBuilder
          .NewCodingTest(d, converter, converter.Encode, x => converter.DecodeUInt64(x))
          .Create(),
      };

      foreach (var action in actions)
      {
        action();
      }
    }

    [Test, TestCaseSource("GetBaseTestData")]
    public void BaseConverter_BaseExpIntsWithSmallVarLexTest(BaseConverter converter)
    {
      var q = Enumerable.Range(0, 64)
        .SelectMany(x => {
          var log = (ulong)Math.Pow(converter.Coding.Base, x);
          return new[] { log - 1, log, log + 1 };
        });

      var a = q.TakeWhile(x => x <= int.MaxValue).Select(x => (int)x);
      var b = q.TakeWhile(x => x <= uint.MaxValue).Select(x => (uint)x);
      var c = q.TakeWhile(x => x <= long.MaxValue).Select(x => (long)x);
      var d = q;

      var actions = new Action[] {
        TestBuilder
          .NewLexicographicOrderTest(a, converter, converter.Encode)
          .Create(),
        TestBuilder
          .NewLexicographicOrderTest(b, converter, converter.Encode)
          .Create(),
        TestBuilder
          .NewLexicographicOrderTest(c, converter, converter.Encode)
          .Create(),
        TestBuilder
          .NewLexicographicOrderTest(d, converter, converter.Encode)
          .Create(),
      };

      foreach (var action in actions)
      {
        action();
      }
    }

    [Test, TestCaseSource("GetBaseTestData")]
    public void BaseConverter_RandomIntsCodingTest(BaseConverter converter)
    {
      var r = Randomness.NextRandom();

      var a = r.NextInt32Sequence().Take(1000);
      var b = r.NextUInt32Sequence().Take(1000);
      var c = r.NextInt64Sequence().Take(1000);
      var d = r.NextUInt64Sequence().Take(1000);

      var actions = new Action[] {
        TestBuilder
          .NewCodingTest(a, converter, converter.Encode, x => converter.DecodeInt32(x))
          .Create(),
        TestBuilder
          .NewCodingTest(b, converter, converter.Encode, x => converter.DecodeUInt32(x))
          .Create(),
        TestBuilder
          .NewCodingTest(c, converter, converter.Encode, x => converter.DecodeInt64(x))
          .Create(),
        TestBuilder
          .NewCodingTest(d, converter, converter.Encode, x => converter.DecodeUInt64(x))
          .Create(),
      };

      foreach (var action in actions)
      {
        action();
      }
    }

    [Test, TestCaseSource("GetBaseTestData")]
    public void BaseConverter_RandomIntsLexTest(BaseConverter converter)
    {
      var r = Randomness.NextRandom();

      var a = r.NextInt32Sequence().Take(1000);
      var b = r.NextUInt32Sequence().Take(1000);
      var c = r.NextInt64Sequence().Take(1000);
      var d = r.NextUInt64Sequence().Take(1000);

      var actions = new Action[] {
        TestBuilder
          .NewLexicographicOrderTest(a, converter, converter.Encode)
          .Create(),
        TestBuilder
          .NewLexicographicOrderTest(b, converter, converter.Encode)
          .Create(),
        TestBuilder
          .NewLexicographicOrderTest(c, converter, converter.Encode)
          .Create(),
        TestBuilder
          .NewLexicographicOrderTest(d, converter, converter.Encode)
          .Create(),
      };

      foreach (var action in actions)
      {
        action();
      }
    }

    [Test, TestCaseSource("GetBaseTestData")]
    public void BaseConverter_RandomBytesCodingTest(BaseConverter converter)
    {
      var r = Randomness.NextRandom();

      var a = Enumerable.Range(0, 1000)
        .Select(x => {
          var v = new byte[r.Next(8)];
          r.NextBytes(v);
          return v;
        });

      var b = Enumerable.Range(0, 1000)
        .Select(x => {
          var v = new byte[r.Next(16)];
          r.NextBytes(v);
          return v;
        });

      var c = Enumerable.Range(0, 1000)
        .Select(x => {
          var v = new byte[r.Next(256)];
          r.NextBytes(v);
          return v;
        });

      var actions = new Action[] {
        TestBuilder
          .NewCodingTest(a, converter, converter.Encode, x => converter.DecodeBytes(x))
          .WithStringFunction(x => string.Join(",", x.Select(y => y.ToString("X2"))))
          .Create(),
        TestBuilder
          .NewCodingTest(b, converter, converter.Encode, x => converter.DecodeBytes(x))
          .WithStringFunction(x => string.Join(",", x.Select(y => y.ToString("X2"))))
          .Create(),
        TestBuilder
          .NewCodingTest(c, converter, converter.Encode, x => converter.DecodeBytes(x))
          .WithStringFunction(x => string.Join(",", x.Select(y => y.ToString("X2"))))
          .Create(),
      };

      foreach (var action in actions)
      {
        action();
      }
    }

    [Test, TestCaseSource("GetBaseTestData")]
    public void BaseConverter_RandomBytesLexTest(BaseConverter converter)
    {
      var r = Randomness.NextRandom();

      var a = Enumerable.Range(0, 1000)
        .Select(x => {
          var v = new byte[r.Next(8)];
          r.NextBytes(v);
          return v;
        });

      var b = Enumerable.Range(0, 1000)
        .Select(x => {
          var v = new byte[r.Next(16)];
          r.NextBytes(v);
          return v;
        });

      var c = Enumerable.Range(0, 1000)
        .Select(x => {
          var v = new byte[r.Next(256)];
          r.NextBytes(v);
          return v;
        });

      var actions = new Action[] {
        TestBuilder
          .NewLexicographicOrderTest(a, converter, converter.Encode)
          .WithComparer(new ByteArrayComparer())
          .WithStringFunction(x => string.Join(",", x.Select(y => y.ToString("X2"))))
          .WithVariableLengthData(true)
          .Create(),
        TestBuilder
          .NewLexicographicOrderTest(b, converter, converter.Encode)
          .WithComparer(new ByteArrayComparer())
          .WithStringFunction(x => string.Join(",", x.Select(y => y.ToString("X2"))))
          .WithVariableLengthData(true)
          .Create(),
        TestBuilder
          .NewLexicographicOrderTest(c, converter, converter.Encode)
          .WithComparer(new ByteArrayComparer())
          .WithStringFunction(x => string.Join(",", x.Select(y => y.ToString("X2"))))
          .WithVariableLengthData(true)
          .Create(),
      };

      foreach (var action in actions)
      {
        action();
      }
    }

    [Test, TestCaseSource("GetBaseTestData")]
    public void BaseConverter_FixedBytesCodingTest(BaseConverter converter)
    {
      var q = new[] {
        new byte[] {},

        new byte[] { 0, },
        new byte[] { 1, },
        new byte[] { 255, },

        new byte[] { 0, 0, },
        new byte[] { 0, 1, },
        new byte[] { 0, 255, },
        new byte[] { 1, 0, },
        new byte[] { 1, 1, },
        new byte[] { 1, 255, },
        new byte[] { 255, 0, },
        new byte[] { 255, 1, },
        new byte[] { 255, 255, },

        new byte[] { 0, 0, 0, },
        new byte[] { 0, 0, 1, },
        new byte[] { 0, 0, 255, },
        new byte[] { 0, 1, 0, },
        new byte[] { 0, 1, 1, },
        new byte[] { 0, 1, 255, },
        new byte[] { 0, 255, 0, },
        new byte[] { 0, 255, 1, },
        new byte[] { 0, 255, 255, },
        new byte[] { 1, 0, 0, },
        new byte[] { 1, 0, 1, },
        new byte[] { 1, 0, 255, },
        new byte[] { 1, 1, 0, },
        new byte[] { 1, 1, 1, },
        new byte[] { 1, 1, 255, },
        new byte[] { 1, 255, 0, },
        new byte[] { 1, 255, 1, },
        new byte[] { 1, 255, 255, },
        new byte[] { 255, 0, 0, },
        new byte[] { 255, 0, 1, },
        new byte[] { 255, 0, 255, },
        new byte[] { 255, 1, 0, },
        new byte[] { 255, 1, 1, },
        new byte[] { 255, 1, 255, },
        new byte[] { 255, 255, 0, },
        new byte[] { 255, 255, 1, },
        new byte[] { 255, 255, 255, },

        new byte[] { 0, 0, 0, 0, },
        new byte[] { 0, 0, 0, 1, },
        new byte[] { 0, 0, 0, 255, },
        new byte[] { 0, 0, 1, 0, },
        new byte[] { 0, 0, 1, 1, },
        new byte[] { 0, 0, 1, 255, },
        new byte[] { 0, 0, 255, 0, },
        new byte[] { 0, 0, 255, 1, },
        new byte[] { 0, 0, 255, 255, },
        new byte[] { 0, 1, 0, 0, },
        new byte[] { 0, 1, 0, 1, },
        new byte[] { 0, 1, 0, 255, },
        new byte[] { 0, 1, 1, 0, },
        new byte[] { 0, 1, 1, 1, },
        new byte[] { 0, 1, 1, 255, },
        new byte[] { 0, 1, 255, 0, },
        new byte[] { 0, 1, 255, 1, },
        new byte[] { 0, 1, 255, 255, },
        new byte[] { 0, 255, 0, 0, },
        new byte[] { 0, 255, 0, 1, },
        new byte[] { 0, 255, 0, 255, },
        new byte[] { 0, 255, 1, 0, },
        new byte[] { 0, 255, 1, 1, },
        new byte[] { 0, 255, 1, 255, },
        new byte[] { 0, 255, 255, 0, },
        new byte[] { 0, 255, 255, 1, },
        new byte[] { 0, 255, 255, 255, },

        new byte[] { 1, 0, 0, 0, },
        new byte[] { 1, 0, 0, 1, },
        new byte[] { 1, 0, 0, 255, },
        new byte[] { 1, 0, 1, 0, },
        new byte[] { 1, 0, 1, 1, },
        new byte[] { 1, 0, 1, 255, },
        new byte[] { 1, 0, 255, 0, },
        new byte[] { 1, 0, 255, 1, },
        new byte[] { 1, 0, 255, 255, },
        new byte[] { 1, 1, 0, 0, },
        new byte[] { 1, 1, 0, 1, },
        new byte[] { 1, 1, 0, 255, },
        new byte[] { 1, 1, 1, 0, },
        new byte[] { 1, 1, 1, 1, },
        new byte[] { 1, 1, 1, 255, },
        new byte[] { 1, 1, 255, 0, },
        new byte[] { 1, 1, 255, 1, },
        new byte[] { 1, 1, 255, 255, },
        new byte[] { 1, 255, 0, 0, },
        new byte[] { 1, 255, 0, 1, },
        new byte[] { 1, 255, 0, 255, },
        new byte[] { 1, 255, 1, 0, },
        new byte[] { 1, 255, 1, 1, },
        new byte[] { 1, 255, 1, 255, },
        new byte[] { 1, 255, 255, 0, },
        new byte[] { 1, 255, 255, 1, },
        new byte[] { 1, 255, 255, 255, },

        new byte[] { 255, 0, 0, 0, },
        new byte[] { 255, 0, 0, 1, },
        new byte[] { 255, 0, 0, 255, },
        new byte[] { 255, 0, 1, 0, },
        new byte[] { 255, 0, 1, 1, },
        new byte[] { 255, 0, 1, 255, },
        new byte[] { 255, 0, 255, 0, },
        new byte[] { 255, 0, 255, 1, },
        new byte[] { 255, 0, 255, 255, },
        new byte[] { 255, 1, 0, 0, },
        new byte[] { 255, 1, 0, 1, },
        new byte[] { 255, 1, 0, 255, },
        new byte[] { 255, 1, 1, 0, },
        new byte[] { 255, 1, 1, 1, },
        new byte[] { 255, 1, 1, 255, },
        new byte[] { 255, 1, 255, 0, },
        new byte[] { 255, 1, 255, 1, },
        new byte[] { 255, 1, 255, 255, },
        new byte[] { 255, 255, 0, 0, },
        new byte[] { 255, 255, 0, 1, },
        new byte[] { 255, 255, 0, 255, },
        new byte[] { 255, 255, 1, 0, },
        new byte[] { 255, 255, 1, 1, },
        new byte[] { 255, 255, 1, 255, },
        new byte[] { 255, 255, 255, 0, },
        new byte[] { 255, 255, 255, 1, },
        new byte[] { 255, 255, 255, 255, },
      };

      var actions = new Action[] {
        TestBuilder
          .NewCodingTest(q, converter, converter.Encode, x => converter.DecodeBytes(x))
          .WithMessage("Cannot encode/decode 'Remainder'.")
          .WithStringFunction(x => string.Join(",", x.Select(y => y.ToString("X2"))))
          .Create(),
      };

      foreach (var action in actions)
      {
        action();
      }
    }
  }
}
