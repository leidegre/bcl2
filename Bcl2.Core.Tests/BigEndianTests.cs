using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bcl2
{
  [TestFixture]
  public class BigEndianTests
  {
    [Test]
    public void BigEndian_ByteSwapUInt16Test()
    {
      var r = Randomness.NextRandom();

      var bytes = new byte[2];
      for (int i = 0; i < 1000; i++)
      {
        r.NextBytes(bytes);
        var reversed = BitConverter.GetBytes(BitConverter.ToUInt16(bytes, 0).ByteSwap());
        CollectionAssert.AreEqual(bytes.Reverse().ToArray(), reversed);
      }
    }

    [Test]
    public void BigEndian_ByteSwapUInt32Test()
    {
      var r = Randomness.NextRandom();

      var bytes = new byte[4];
      for (int i = 0; i < 1000; i++)
      {
        r.NextBytes(bytes);
        var reversed = BitConverter.GetBytes(BitConverter.ToUInt32(bytes, 0).ByteSwap());
        CollectionAssert.AreEqual(bytes.Reverse().ToArray(), reversed);
      }
    }

    [Test]
    public void BigEndian_ByteSwapUInt64Test()
    {
      var r = Randomness.NextRandom();

      var bytes = new byte[8];
      for (int i = 0; i < 1000; i++)
      {
        r.NextBytes(bytes);
        var reversed = BitConverter.GetBytes(BitConverter.ToUInt64(bytes, 0).ByteSwap());
        CollectionAssert.AreEqual(bytes.Reverse().ToArray(), reversed);
      }
    }
  }
}
