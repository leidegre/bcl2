using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bcl2.PositionalNotation
{
  [TestFixture]
  public class PositionalNotationTests
  {
    static class TestBuilder
    {
      public class CodingTest<T>
      {
        private readonly IEnumerable<T> _source;
        private readonly Func<T, string> _encoder;
        private readonly Func<string, T> _decoder;
        private string _message;
        private Func<T, string> _strFunc;

        public CodingTest(IEnumerable<T> source, Func<T, string> encoder, Func<string, T> decoder)
        {
          this._source = source;
          this._encoder = encoder;
          this._decoder = decoder;
        }

        public CodingTest<T> WithMessage(string message)
        {
          this._message = message;
          return this;
        }

        public CodingTest<T> WithStringFunction(Func<T, string> strFunc)
        {
          this._strFunc = strFunc;
          return this;
        }

        public Action Create()
        {
          return () => {
            var list = _source.ToList();

            var q =
              from v in list
              let s = _encoder(v)
              let y = _decoder(s)
              select Tuple.Create(v, s, y);

            var message = string.Format(_message ?? "Cannot encoder/decode '{0}'", typeof(T));

            int i = 0;
            foreach (var item in q)
            {
              Assert.AreEqual(item.Item1, item.Item3, message);
#if DEBUG
              if ((i != 0) & ((i % 100) == 0))
              {
                var s = _strFunc != null ? _strFunc(item.Item1) : item.Item1.ToString();
                Trace.WriteLine(s + " <-> " + item.Item2);
              }
#endif
              i++;
            }
          };
        }
      }

      public static CodingTest<T> NewCodingTest<T>(IEnumerable<T> source, Func<T, string> encoder, Func<string, T> decoder)
      {
        return new CodingTest<T>(source, encoder, decoder);
      }

      public class LexicographicTest<T>
      {
        private readonly IEnumerable<T> _source;
        private readonly Func<T, string> _encoder;
        private IComparer<T> _comparer;
        private string _message;
        private Func<T, string> _strFunc;

        public LexicographicTest(IEnumerable<T> source, Func<T, string> encoder)
        {
          this._source = source;
          this._encoder = encoder;
        }

        public LexicographicTest<T> WithComparer(IComparer<T> comparer)
        {
          this._comparer = comparer;
          return this;
        }

        public LexicographicTest<T> WithMessage(string message)
        {
          this._message = message;
          return this;
        }

        public LexicographicTest<T> WithStringFunction(Func<T, string> strFunc)
        {
          this._strFunc = strFunc;
          return this;
        }

        public Action Create()
        {
          return () => {
            var list = _source.ToList();

            var q =
              from v in list
              let s = _encoder(v)
              select Tuple.Create(v, s);

            var a = q.OrderBy(x => x.Item1, _comparer ?? Comparer<T>.Default).ToList();
            var b = q.OrderBy(x => x.Item2, StringComparer.Ordinal).ToList();

            int i = 0;
            foreach (var item in b)
            {
#if DEBUG
              if ((i != 0) & ((i % 100) == 0))
              {
                var s = _strFunc != null ? _strFunc(item.Item1) : item.Item1.ToString();
                Trace.WriteLine(s + " -> " + item.Item2);
              }
#endif
              i++;
            }

            var message = string.Format(_message ?? "Lexicographic ordering is not preserved for '{0}'", typeof(T));

            CollectionAssert.AreEqual(a, b, message);
          };
        }
      }

      public static LexicographicTest<T> NewLexicographicOrderTest<T>(IEnumerable<T> source, Func<T, string> encoder)
      {
        return new LexicographicTest<T>(source, encoder);
      }
    }

    public IEnumerable<BaseConverter> GetBaseTestData()
    {
      yield return BaseConvert.Binary;
      yield return BaseConvert.Decimal;
      yield return BaseConvert.Hexadecimal;
      yield return BaseConvert.Triacontakaidecimal;
      yield return BaseConvert.Crockford;
      yield return BaseConvert.Base62;
    }

    [Test, TestCaseSource("GetBaseTestData")]
    public void PositionalNotation_IntBaseExpWithSmallVarCodingTest(BaseConverter converter)
    {
      var q = Enumerable.Range(0, 64)
        .SelectMany(x => {
          var log = (ulong)Math.Pow(converter.Base, x);
          return new[] { log - 1, log, log + 1 };
        });

      var a = q.TakeWhile(x => x <= byte.MaxValue).Select(x => (byte)x);
      var b = q.TakeWhile(x => x <= ushort.MaxValue).Select(x => (ushort)x);
      var c = q.TakeWhile(x => x <= int.MaxValue).Select(x => (int)x);
      var d = q.TakeWhile(x => x <= uint.MaxValue).Select(x => (uint)x);
      var e = q.TakeWhile(x => x <= long.MaxValue).Select(x => (long)x);
      var f = q;

      var actions = new Action[] {
        TestBuilder
          .NewCodingTest(a, converter.Encode, x => converter.DecodeByte(x, 0))
          .Create(),
        TestBuilder
          .NewCodingTest(b, converter.Encode, x => converter.DecodeUInt16(x, 0))
          .Create(),
        TestBuilder
          .NewCodingTest(c, converter.Encode, x => converter.DecodeInt32(x, 0))
          .Create(),
        TestBuilder
          .NewCodingTest(d, converter.Encode, x => converter.DecodeUInt32(x, 0))
          .Create(),
        TestBuilder
          .NewCodingTest(e, converter.Encode, x => converter.DecodeInt64(x, 0))
          .Create(),
        TestBuilder
          .NewCodingTest(f, converter.Encode, x => converter.DecodeUInt64(x, 0))
          .Create(),
      };

      foreach (var action in actions)
      {
        action();
      }
    }

    [Test, TestCaseSource("GetBaseTestData")]
    public void PositionalNotation_IntBaseExpWithSmallVarLexTest(BaseConverter converter)
    {
      var q = Enumerable.Range(0, 64)
        .SelectMany(x => {
          var log = (ulong)Math.Pow(converter.Base, x);
          return new[] { log - 1, log, log + 1 };
        });

      var a = q.TakeWhile(x => x <= byte.MaxValue).Select(x => (byte)x);
      var b = q.TakeWhile(x => x <= ushort.MaxValue).Select(x => (ushort)x);
      var c = q.TakeWhile(x => x <= int.MaxValue).Select(x => (int)x);
      var d = q.TakeWhile(x => x <= uint.MaxValue).Select(x => (uint)x);
      var e = q.TakeWhile(x => x <= long.MaxValue).Select(x => (long)x);
      var f = q;

      var actions = new Action[] {
        TestBuilder
          .NewLexicographicOrderTest(a, converter.Encode)
          .Create(),
        TestBuilder
          .NewLexicographicOrderTest(b, converter.Encode)
          .Create(),
        TestBuilder
          .NewLexicographicOrderTest(c, converter.Encode)
          .Create(),
        TestBuilder
          .NewLexicographicOrderTest(d, converter.Encode)
          .Create(),
        TestBuilder
          .NewLexicographicOrderTest(e, converter.Encode)
          .Create(),
        TestBuilder
          .NewLexicographicOrderTest(f, converter.Encode)
          .Create(),
      };

      foreach (var action in actions)
      {
        action();
      }
    }

    [Test, TestCaseSource("GetBaseTestData")]
    public void PositionalNotation_IntRandomCodingTest(BaseConverter converter)
    {
      var r = Randomness.NextRandom();

      var a = r.NextByteSequence().Take(1000);
      var b = r.NextUInt16Sequence().Take(1000);
      var c = r.NextInt32Sequence().Take(1000);
      var d = r.NextUInt32Sequence().Take(1000);
      var e = r.NextInt64Sequence().Take(1000);
      var f = r.NextUInt64Sequence().Take(1000);

      var actions = new Action[] {
        TestBuilder
          .NewCodingTest(a, converter.Encode, x => converter.DecodeByte(x, 0))
          .Create(),
        TestBuilder
          .NewCodingTest(b, converter.Encode, x => converter.DecodeUInt16(x, 0))
          .Create(),
        TestBuilder
          .NewCodingTest(c, converter.Encode, x => converter.DecodeInt32(x, 0))
          .Create(),
        TestBuilder
          .NewCodingTest(d, converter.Encode, x => converter.DecodeUInt32(x, 0))
          .Create(),
        TestBuilder
          .NewCodingTest(e, converter.Encode, x => converter.DecodeInt64(x, 0))
          .Create(),
        TestBuilder
          .NewCodingTest(f, converter.Encode, x => converter.DecodeUInt64(x, 0))
          .Create(),
      };

      foreach (var action in actions)
      {
        action();
      }
    }

    [Test, TestCaseSource("GetBaseTestData")]
    public void PositionalNotation_IntRandomLexTest(BaseConverter converter)
    {
      var r = Randomness.NextRandom();

      var a = r.NextByteSequence().Take(1000);
      var b = r.NextUInt16Sequence().Take(1000);
      var c = r.NextInt32Sequence().Take(1000);
      var d = r.NextUInt32Sequence().Take(1000);
      var e = r.NextInt64Sequence().Take(1000);
      var f = r.NextUInt64Sequence().Take(1000);

      var actions = new Action[] {
        TestBuilder
          .NewLexicographicOrderTest(a, converter.Encode)
          .Create(),
        TestBuilder
          .NewLexicographicOrderTest(b, converter.Encode)
          .Create(),
        TestBuilder
          .NewLexicographicOrderTest(c, converter.Encode)
          .Create(),
        TestBuilder
          .NewLexicographicOrderTest(d, converter.Encode)
          .Create(),
        TestBuilder
          .NewLexicographicOrderTest(e, converter.Encode)
          .Create(),
        TestBuilder
          .NewLexicographicOrderTest(f, converter.Encode)
          .Create(),
      };

      foreach (var action in actions)
      {
        action();
      }
    }

    [Test, TestCaseSource("GetBaseTestData")]
    public void PositionalNotation_BytesRandomCodingTest(BaseConverter converter)
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
          var v = new byte[r.Next(32)];
          r.NextBytes(v);
          return v;
        });

      var actions = new Action[] {
        TestBuilder
          .NewCodingTest(a, converter.Encode, x => converter.DecodeBytes(x))
          .WithStringFunction(x => string.Join(",", x.Select(y => y.ToString("X2"))))
          .Create(),
        TestBuilder
          .NewCodingTest(b, converter.Encode, x => converter.DecodeBytes(x))
          .WithStringFunction(x => string.Join(",", x.Select(y => y.ToString("X2"))))
          .Create(),
        TestBuilder
          .NewCodingTest(c, converter.Encode, x => converter.DecodeBytes(x))
          .WithStringFunction(x => string.Join(",", x.Select(y => y.ToString("X2"))))
          .Create(),
      };

      foreach (var action in actions)
      {
        action();
      }
    }

    [Test, TestCaseSource("GetBaseTestData")]
    public void PositionalNotation_BytesRandomLexTest(BaseConverter converter)
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
          var v = new byte[r.Next(32)];
          r.NextBytes(v);
          return v;
        });

      var actions = new Action[] {
        TestBuilder
          .NewLexicographicOrderTest(a, converter.Encode)
          .WithComparer(new ByteArrayComparer())
          .WithStringFunction(x => string.Join(",", x.Select(y => y.ToString("X2"))))
          .Create(),
        TestBuilder
          .NewLexicographicOrderTest(b, converter.Encode)
          .WithComparer(new ByteArrayComparer())
          .WithStringFunction(x => string.Join(",", x.Select(y => y.ToString("X2"))))
          .Create(),
        TestBuilder
          .NewLexicographicOrderTest(c, converter.Encode)
          .WithComparer(new ByteArrayComparer())
          .WithStringFunction(x => string.Join(",", x.Select(y => y.ToString("X2"))))
          .Create(),
      };

      foreach (var action in actions)
      {
        action();
      }
    }

    [Test, TestCaseSource("GetBaseTestData")]
    public void PositionalNotation_BytesRemainderCodingTest(BaseConverter converter)
    {
      var bytes = new byte[] { 0, 1, 254, 255 };

      var r = Randomness.NextRandom();

      var q = Enumerable.Range(0, 1000)
        .Select(_ => {
          var length = r.Next(8);
          var x = new byte[length];
          for (int i = 0; i < length; i++)
          {
            x[i] = r.NextElement(bytes);
          }
          return x;
        });

      var actions = new Action[] {
        TestBuilder
          .NewCodingTest(q, converter.EncodeRemainder, x => converter.DecodeRemainder(x))
          .WithMessage("Cannot encode/decode 'Remainder'.")
          .WithStringFunction(x => string.Join(",", x.Select(y => y.ToString("X2"))))
          .Create(),
      };

      foreach (var action in actions)
      {
        action();
      }
    }

    [Test, TestCaseSource("GetBaseTestData")]
    public void PositionalNotation_BytesRemainderLexTest(BaseConverter converter)
    {
      var bytes = new byte[] { 0, 1, 254, 255 };

      var r = Randomness.NextRandom();

      var q = Enumerable.Range(0, 1000)
        .Select(_ => {
          var length = r.Next(8);
          var x = new byte[length];
          for (int i = 0; i < length; i++)
          {
            x[i] = r.NextElement(bytes);
          }
          return x;
        });

      var actions = new Action[] {
        TestBuilder
          .NewLexicographicOrderTest(q, converter.EncodeRemainder)
          .WithComparer(new ByteArrayComparer())
          .WithMessage("Lexicographic ordering is not preserved for 'Remainder'")
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
