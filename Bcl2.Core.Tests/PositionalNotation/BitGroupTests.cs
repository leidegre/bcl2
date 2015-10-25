using NUnit.Framework;
using System;

namespace Bcl2.PositionalNotation
{
  [TestFixture]
  public class BitGroupTests
  {
    [Test]
    public void BitGroup_LoadFromTest()
    {
      var xs = new[] {
        Tuple.Create(new byte[] {  }, 0x00UL),
        Tuple.Create(new byte[] { 0x01 }, 0x01UL),
        Tuple.Create(new byte[] { 0x01, 0x02 }, 0x0102UL),
        Tuple.Create(new byte[] { 0x01, 0x02, 0x03 }, 0x010203UL),
        Tuple.Create(new byte[] { 0x01, 0x02, 0x03, 0x04 }, 0x01020304UL),
        Tuple.Create(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 }, 0x0102030405UL),
        Tuple.Create(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 }, 0x010203040506UL),
        Tuple.Create(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 }, 0x01020304050607UL),
        Tuple.Create(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 }, 0x0102030405060708UL),
      };

      foreach (var x in xs)
      {
        var bitGroup = new BitGroup();
        bitGroup.LoadFrom(x.Item1, 0, x.Item1.Length);
        Assert.AreEqual(x.Item2, bitGroup.Value);
      }
    }

    [Test]
    public void BitGroup_ReadByteArrayTest()
    {
      var xs = new[] {
        Tuple.Create(new byte[] {  }, 0x00UL),
        Tuple.Create(new byte[] { 0x01 }, 0x01UL),
        Tuple.Create(new byte[] { 0x01, 0x02 }, 0x0102UL),
        Tuple.Create(new byte[] { 0x01, 0x02, 0x03 }, 0x010203UL),
        Tuple.Create(new byte[] { 0x01, 0x02, 0x03, 0x04 }, 0x01020304UL),
        Tuple.Create(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 }, 0x0102030405UL),
        Tuple.Create(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 }, 0x010203040506UL),
        Tuple.Create(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 }, 0x01020304050607UL),
        Tuple.Create(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 }, 0x0102030405060708UL),
      };

      foreach (var x in xs)
      {
        var bitGroup = new BitGroup();
        bitGroup.LoadFrom(x.Item1, 0, x.Item1.Length);
        var temp = new byte[x.Item1.Length];
        bitGroup.Read(temp, 0, temp.Length);
        CollectionAssert.AreEqual(x.Item1, temp);
      }
    }
  }
}
