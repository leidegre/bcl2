using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Bcl2.PositionalNotation
{
  [TestFixture]
  public partial class BaseConverterTests
  {
    struct TestVector<T>
    {
      public readonly T Value;
      public readonly string Expected;

      public TestVector(T value, string expected)
      {
        Value = value;
        Expected = expected;
      }
    }

    static class TestVector
    {
      public static TestVector<T> Create<T>(T value, string expected)
      {
        return new TestVector<T>(value, expected);
      }
    }

    static class TestBuilder
    {
      public class CodingTest<T>
      {
        private readonly IEnumerable<T> _source;
        private readonly BaseConverter _converter;
        private readonly Func<T, string> _encoder;
        private readonly Func<string, T> _decoder;
        private string _message;
        private Func<T, string> _strFunc;

        public CodingTest(IEnumerable<T> source, BaseConverter converter, Func<T, string> encoder, Func<string, T> decoder)
        {
          this._source = source;
          this._converter = converter;
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

        private string Translate(Random r, string s)
        {
          if (!_converter.Coding.Alphabet.IsCaseSensitive)
          {
            switch (r.Next(3))
            {
              case 0:
                return s.ToLowerInvariant();
              case 1:
                return s;
              case 2:
                return s.ToUpperInvariant();
            }
          }
          return s;
        }

        public Action Create()
        {
          return () => {
            var list = _source.ToList();
            var r = Randomness.NextRandom();

            var q =
              from v in list
              let s = _encoder(v)
              let t = Translate(r, s)
              let y = _decoder(t)
              select Tuple.Create(v, s, y);

            var message = string.Format(_message ?? "Cannot encoder/decode '{0}'", typeof(T));

            int i = 0;
            foreach (var item in q)
            {
#if DEBUG
              if ((i != 0) & ((i % 100) == 0))
              {
                var s = _strFunc != null ? _strFunc(item.Item1) : item.Item1.ToString();
                Trace.WriteLine(s + " <-> " + item.Item2);
              }
#endif
              Assert.AreEqual(item.Item1, item.Item3, message);
              i++;
            }
          };
        }
      }

      public static CodingTest<T> NewCodingTest<T>(IEnumerable<T> source, BaseConverter converter, Func<T, string> encoder, Func<string, T> decoder)
      {
        return new CodingTest<T>(source, converter, encoder, decoder);
      }

      public class LexicographicTest<T>
      {
        private readonly IEnumerable<T> _source;
        private readonly BaseConverter _converter;
        private readonly Func<T, string> _encoder;
        private IComparer<T> _comparer;
        private string _message;
        private Func<T, string> _strFunc;
        private bool _hasVariableLengthData;

        public LexicographicTest(IEnumerable<T> source, BaseConverter converter, Func<T, string> encoder)
        {
          this._source = source;
          this._converter = converter;
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

        public LexicographicTest<T> WithVariableLengthData(bool hasVariableLengthData)
        {
          this._hasVariableLengthData = hasVariableLengthData;
          return this;
        }

        public Action Create()
        {
          return () => {
            if (!_converter.Coding.Alphabet.IsLexicographic)
            {
              Assert.Pass("Alphabet does not support lexicographical order.");
            }
            if (_hasVariableLengthData & !_converter.Coding.IsLexicographic)
            {
              Assert.Pass("Coding does not support lexicographical order.");
            }

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

      public static LexicographicTest<T> NewLexicographicOrderTest<T>(IEnumerable<T> source, BaseConverter converter, Func<T, string> encoder)
      {
        return new LexicographicTest<T>(source, converter, encoder);
      }

      public sealed class TestVectorTest<T>
      {
        private readonly IEnumerable<TestVector<T>> _source;
        private readonly BaseConverter _converter;
        private readonly Func<T, string> _encoder;
        private string _message;
        private Func<T, string> _strFunc;

        public TestVectorTest(IEnumerable<TestVector<T>> source, BaseConverter converter, Func<T, string> encoder)
        {
          this._source = source;
          this._converter = converter;
          this._encoder = encoder;
        }

        public TestVectorTest<T> WithMessage(string message)
        {
          this._message = message;
          return this;
        }

        public TestVectorTest<T> WithStringFunction(Func<T, string> strFunc)
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
              let actual = _encoder(v.Value)
              select Tuple.Create(v.Value, actual, v.Expected);

            var message = string.Format(_message ?? "Cannot encode '{0}'", typeof(T));

            int i = 0;
            foreach (var item in q)
            {
#if DEBUG
              if ((i != 0) & ((i % 100) == 0))
              {
                var s = _strFunc != null ? _strFunc(item.Item1) : item.Item1.ToString();
                Trace.WriteLine(s + " -> " + item.Item2);
              }
#endif
              Assert.AreEqual(item.Item3, item.Item2, message);
              i++;
            }
          };
        }
      }

      public static TestVectorTest<T> NewTestVectorTest<T>(IEnumerable<TestVector<T>> source, BaseConverter converter, Func<T, string> encoder)
      {
        return new TestVectorTest<T>(source, converter, encoder);
      }
    }
  }
}
