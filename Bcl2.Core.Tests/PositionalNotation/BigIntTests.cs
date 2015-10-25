using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bcl2.PositionalNotation
{
  [TestFixture]
  public class BigIntTests
  {
    [Test]
    public void BigInt_MulAddDivRemTest()
    {
      const int WordBitSize = 23;

      var r = Randomness.NextRandom();

      for (int i = 0; i < 1000; i++)
      {
        var list = new List<uint>();

        var bigInt = BigInt.FromByteCount(256);

        var q = r
          .NextUInt32Sequence()
          .Select(x => x & ((1U << WordBitSize) - 1))
          .Take(1 + r.Next(64));

        foreach (var x in q)
        {
          bigInt.Mul(1U << WordBitSize);
          bigInt.Add(x);

          list.Add(x);
        }

        list.Reverse();

        foreach (var x in list)
        {
          uint y = bigInt.DivRem(1U << WordBitSize);
          Assert.AreEqual(x, y);
        }
      }
    }
  }
}
