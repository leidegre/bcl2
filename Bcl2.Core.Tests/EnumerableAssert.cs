using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bcl2
{
  static class EnumerableAssert
  {
    public static void AreEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, IEqualityComparer<T> equalityComparer = null, Func<T, string> toString = null)
    {
      var equalityComparer2 = equalityComparer ?? EqualityComparer<T>.Default;
      var toString2 = toString ?? new Func<T, string>(x => Convert.ToString(x));
      int index = 0;
      using (var expectedIt = expected.GetEnumerator())
      {
        using (var actualIt = actual.GetEnumerator())
        {
          bool movedExpected, movedActual;
          while ((movedExpected = expectedIt.MoveNext()) & (movedActual = actualIt.MoveNext()))
          {
            if (!equalityComparer2.Equals(expectedIt.Current, actualIt.Current))
            {
              var expectedValue = toString2(expectedIt.Current);
              var actualValue = toString2(actualIt.Current);
              Assert.Fail("Values differ at index [" + index + "]. Expected is <" + expectedValue + ">, actual is <" + actualValue + ">.");
            }
            index++;
          }
          if (movedExpected ^ movedActual)
          {
            if (movedActual)
            {
              Assert.Fail("Actual sequence is longer than expected sequence.");
            }
            if (movedExpected)
            {
              Assert.Fail("Actual sequence is shorter than expected sequence.");
            }
          }
        }
      }
    }
  }
}
