using NUnit.Framework;
using System;

namespace Bcl2.PositionalNotation
{
  [TestFixture]
  public class AlphabetTests
  {
    private readonly Alphabet ABC = new Alphabet("abc");

    [Test]
    public void Alphabet_EncodeDecodeTest()
    {
      Assert.AreEqual('a', ABC.Encode(0));
      Assert.AreEqual('b', ABC.Encode(1));
      Assert.AreEqual('c', ABC.Encode(2));

      Assert.AreEqual(0, ABC.Decode('a'));
      Assert.AreEqual(1, ABC.Decode('b'));
      Assert.AreEqual(2, ABC.Decode('c'));
    }

    // note that the run-time does throw an IndexOutOfRange, not ArgumentOutOfRange exception here
    [Test, ExpectedException(typeof(IndexOutOfRangeException))]
    public void Alphabet_EncodeOutOfRangeTest()
    {
      ABC.Encode(3);
    }

    [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Alphabet_DecodeOutOfRangeTest()
    {
      ABC.Decode('d');
    }

    private readonly Alphabet ToLower = new Alphabet("abc", "ABC", "abc");

    [Test]
    public void Alphabet_EncodeDecodeToLowerTest()
    {
      // the ToLower alphabet will decode upper case ABC as lower case abc

      Assert.AreEqual('a', ToLower.Encode(0));
      Assert.AreEqual('b', ToLower.Encode(1));
      Assert.AreEqual('c', ToLower.Encode(2));

      Assert.AreEqual(0, ToLower.Decode('A'));
      Assert.AreEqual(1, ToLower.Decode('B'));
      Assert.AreEqual(2, ToLower.Decode('C'));
    }
  }
}
