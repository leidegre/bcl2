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
    public IEnumerable<BaseConverter> GetBaseTestData()
    {
      yield return BaseConvert.Binary;
      yield return BaseConvert.Decimal;
      yield return BaseConvert.Hexadecimal;
      yield return BaseConvert.Triacontakaidecimal;
      yield return BaseConvert.Base62;
    }

    [TestCaseSource("GetBaseTestData")]
    public void PositionalNotation_IntegerCodingTest(BaseConverter converter)
    {
      var r = Randomness.NextRandom();

      for (int i = 0; i < 1000; i++)
      {
        // Byte
        {
          var x = (byte)r.Next(byte.MaxValue + 1);
          var s = converter.Encode(x);
          var y = converter.DecodeByte(s);
          Assert.AreEqual(x, y, "Encode/DecodeByte error");
        }

        // UInt16
        {
          var x = (ushort)r.Next(ushort.MaxValue + 1);
          var s = converter.Encode(x);
          var y = converter.DecodeUInt16(s);
          Assert.AreEqual(x, y, "Encode/DecodeUInt16 error");
        }

        // Int32
        {
          var x = r.Next() - (int.MaxValue / 2);
          var s = converter.Encode(x);
          var y = converter.DecodeInt32(s);
          Assert.AreEqual(x, y, "Encode/DecodeInt32 error");
        }

        // UInt32
        {
          var x = (uint)r.Next();
          var s = converter.Encode(x);
          var y = converter.DecodeUInt32(s);
          Assert.AreEqual(x, y, "Encode/DecodeUInt32 error");
        }

        // Int64
        {
          var x = r.Next64() - (long.MaxValue / 2);
          var s = converter.Encode(x);
          var y = converter.DecodeInt64(s);
          Assert.AreEqual(x, y, "Encode/DecodeInt64 error");
        }

        // UInt64
        {
          var x = (ulong)r.Next64();
          var s = converter.Encode(x);
          var y = converter.DecodeUInt64(s);
          Assert.AreEqual(x, y, "Encode/DecodeUInt64 error");
        }
      }
    }

    [TestCaseSource("GetBaseTestData")]
    public void PositionalNotation_LexicographicIntegerCodingTest(BaseConverter converter)
    {
      var r = Randomness.NextRandom();

      var list = new List<Tuple<int, string>>(1000);
      for (int i = 0; i < 1000; i++)
      {
        var v = r.Next(int.MinValue, int.MaxValue);
        var s = converter.Encode(v);
        list.Add(Tuple.Create(v, s));
      }

      var list2 = list.OrderBy(x => x.Item1).ToList();
      var list3 = list.OrderBy(x => x.Item2, StringComparer.Ordinal).ToList();

      CollectionAssert.AreEqual(list2, list3);
    }

    [Test]
    public void PositionalNotation_ByteSwapTest()
    {
      var r = Randomness.NextRandom();

      // UInt16
      {
        var bytes = new byte[2];
        for (int i = 0; i < 1000; i++)
        {
          r.NextBytes(bytes);
          var reversed = BitConverter.GetBytes(BitConverter.ToUInt16(bytes, 0).ByteSwap());
          CollectionAssert.AreEqual(bytes.Reverse().ToArray(), reversed);
        }
      }

      // UInt32
      {
        var bytes = new byte[4];
        for (int i = 0; i < 1000; i++)
        {
          r.NextBytes(bytes);
          var reversed = BitConverter.GetBytes(BitConverter.ToUInt32(bytes, 0).ByteSwap());
          CollectionAssert.AreEqual(bytes.Reverse().ToArray(), reversed);
        }
      }

      // UInt64
      {
        var bytes = new byte[8];
        for (int i = 0; i < 1000; i++)
        {
          r.NextBytes(bytes);
          var reversed = BitConverter.GetBytes(BitConverter.ToUInt64(bytes, 0).ByteSwap());
          CollectionAssert.AreEqual(bytes.Reverse().ToArray(), reversed);
        }
      }
    }

    [TestCaseSource("GetBaseTestData")]
    public void PositionalNotation_BytesCodingTest(BaseConverter converter)
    {
      var r = Randomness.NextRandom();

      var bytes = new byte[32];
      for (int i = 0; i < 1000; i++)
      {
        var offset = r.Next(32);
        var length = r.Next(32 - offset);
        r.NextBytes(bytes);
        var s = converter.Encode(bytes, offset, length);
        var x = converter.DecodeBytes(s);
        CollectionAssert.AreEqual(bytes.Skip(offset).Take(length).ToArray(), x);
      }
    }

    [TestCaseSource("GetBaseTestData")]
    public void PositionalNotation_LexicographicBytesCodingTest(BaseConverter converter)
    {
      var r = Randomness.NextRandom();

      var list = new List<Tuple<byte[], string>>(1000);
      for (int i = 0; i < 1000; i++)
      {
        var bytes = new byte[r.Next(16)];
        r.NextBytes(bytes);
        var s = converter.Encode(bytes);
        list.Add(Tuple.Create(bytes, s));
      }

      var list2 = list.OrderBy(x => x.Item1, new ByteComparer()).ToList();
      var list3 = list.OrderBy(x => x.Item2, StringComparer.Ordinal).ToList();

      for (int i = 0; i < 1000; i++)
      {
        var item2 = list2[i];
        Trace.WriteLine(i.ToString("0000") + ": " + BitConverter.ToString(item2.Item1) + " <-> " + item2.Item2);

        var item3 = list3[i];
        Trace.WriteLine(i.ToString("0000") + ": " + BitConverter.ToString(item3.Item1) + " <-> " + item3.Item2);
      }

      CollectionAssert.AreEqual(list2, list3);
    }

    [Test]
    public void PositionalNotation_Exp()
    {
      // here's an issue with bases that don't wrap on a even byte boundary
      // this is not an issue with bases 2 and 16

      var x = BaseConvert.Base62;

      //var ys = Enumerable.Range(0, 7)
      //  .SelectMany(y => {
      //    var yy = (ulong)Math.Pow(62, y);
      //    return new[] { yy - 1, yy, yy + 1 };
      //  });

      //foreach (var y in ys)
      //{
      //  var a = x.Encode((byte)y);
      //  var b = x.Encode((ushort)y);
      //  var c = x.Encode((uint)y);
      //  var d = x.Encode((ulong)y);

      //  Trace.WriteLine(a + ", " + b + ", " + c + ", " + d);
      //}

      var ys = new[] {
        new byte[] { 0 },
        new byte[] { 1 },
        new byte[] { 255 },
        new byte[] { 1, 0 },
        new byte[] { 1, 1 },
        new byte[] { 1, 255 },
        new byte[] { 2, 0 },
        new byte[] { 2, 1 },
        new byte[] { 2, 255 },
        new byte[] { 3, 0, 0 },
        new byte[] { 3, 0, 1 },
        new byte[] { 3, 0, 255 },
        new byte[] { 3, 1, 0 },
        new byte[] { 3, 1, 1 },
        new byte[] { 3, 1, 255 },
        new byte[] { 3, 2, 0 },
        new byte[] { 3, 2, 1 },
        new byte[] { 3, 2, 255 },
        new byte[] { 4, 0, 0, 0 },
        new byte[] { 4, 0, 0, 1 },
        new byte[] { 4, 0, 0, 255 },
        new byte[] { 4, 0, 1, 0 },
        new byte[] { 4, 0, 1, 1 },
        new byte[] { 4, 0, 1, 255 },
        new byte[] { 4, 1, 0, 0 },
        new byte[] { 4, 1, 0, 1 },
        new byte[] { 4, 1, 0, 255 },
      };

      foreach (var y in ys)
      {
        var a = string.Join(", ", y.Select(z => z.ToString("000")));
        var b = x.Encode(y);

        Trace.WriteLine(a + " <-> " + b);
      }
    }
  }
}
