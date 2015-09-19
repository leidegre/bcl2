using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bcl2
{
  [TestFixture]
  public class ByteComparerTests
  {
    [Test]
    public void ByteComparer_BaseCaseTest()
    {
      var comparer = new ByteComparer();

      {
        var a = new byte[0];
        var b = new byte[0];
        Assert.IsTrue(comparer.Compare(a, b) == 0);
        Assert.IsTrue(comparer.Compare(b, a) == 0);
      }

      {
        var a = new byte[0];
        var b = new byte[1];
        Assert.IsTrue(comparer.Compare(a, b) < 0);
        Assert.IsTrue(comparer.Compare(b, a) > 0);
      }

      {
        var a = new byte[] { 1 };
        var b = new byte[] { 2 };
        Assert.IsTrue(comparer.Compare(a, b) < 0);
        Assert.IsTrue(comparer.Compare(b, a) > 0);
      }

      {
        var a = new byte[] { 3 };
        var b = new byte[] { 3 };
        Assert.IsTrue(comparer.Compare(a, b) == 0);
        Assert.IsTrue(comparer.Compare(b, a) == 0);
      }

      {
        var a = new byte[] { 1, 2 };
        var b = new byte[] { 1, 3 };
        Assert.IsTrue(comparer.Compare(a, b) < 0);
        Assert.IsTrue(comparer.Compare(b, a) > 0);
      }

      {
        var a = new byte[] { 1, 2 };
        var b = new byte[] { 1, 2 };
        Assert.IsTrue(comparer.Compare(a, b) == 0);
        Assert.IsTrue(comparer.Compare(b, a) == 0);
      }

      {
        var a = new byte[] { 1, 2, 3 };
        var b = new byte[] { 1, 2, 4 };
        Assert.IsTrue(comparer.Compare(a, b) < 0);
        Assert.IsTrue(comparer.Compare(b, a) > 0);
      }

      {
        var a = new byte[] { 1, 2, 3 };
        var b = new byte[] { 1, 2, 3 };
        Assert.IsTrue(comparer.Compare(a, b) == 0);
        Assert.IsTrue(comparer.Compare(b, a) == 0);
      }
    }

    [Test]
    public void ByteComparer_SortRandomUInt32Test()
    {
      var comparer = new ByteComparer();

      var r = Randomness.NextRandom();

      var list = new List<byte[]>(1000);
      var list2 = new List<uint>(1000);
      for (int i = 0; i < 1000; i++)
      {
        var bytes = new byte[4];
        r.NextBytes(bytes);
        uint x = ((uint)bytes[0] << 24)
           | ((uint)bytes[1] << 16)
           | ((uint)bytes[2] << 8)
           | bytes[3]
          ;
        list.Add(bytes);
        list2.Add(x);
      }

      list.Sort(comparer);
      list2.Sort(); // use default comparer

      CollectionAssert.AreEqual(list2, list.Select(bytes => {
        uint x = ((uint)bytes[0] << 24)
           | ((uint)bytes[1] << 16)
           | ((uint)bytes[2] << 8)
           | bytes[3]
          ;
        return x;
      }).ToList());
    }
  }
}
