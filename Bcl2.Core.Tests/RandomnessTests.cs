using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Bcl2
{
  [TestFixture]
  public class RandomnessTests
  {
    [Test]
    public void Randomness_SecureDoubleTest()
    {
      for (int k = 0; k < 1000; k++)
      {
        var list = new List<double>();

        for (int i = 0; i < 1000; i++)
        {
          list.Add(Randomness.NextSecureDouble());
        }

        var avg = list.Average();

        Assert.AreEqual(0.5, avg, 0.05);
      }
    }
  }
}
