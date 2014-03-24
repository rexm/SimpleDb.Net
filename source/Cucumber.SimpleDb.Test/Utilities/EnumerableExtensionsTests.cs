using System.Collections.Generic;
using System.Linq;
using Cucumber.SimpleDb.Utilities;
using NUnit.Framework;

namespace Cucumber.SimpleDb.Test
{
    [TestFixture]
    public class EnumerableExtensionsTests
    {
        [Test]
        public void TwoAndAHalfGroupsOfFive()
        {
            var numbers = new int[13]
            {
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13
            };
            var chunkedNumbers = numbers.GroupsOf(5);
            Assert.AreEqual(3, chunkedNumbers.Count());
            var chunkedNumbersList = chunkedNumbers.ToList();
            Assert.IsInstanceOf<IEnumerable<int>>(chunkedNumbersList[0]);
            Assert.AreEqual(5, chunkedNumbersList[0].Count());
            Assert.AreEqual(5, chunkedNumbersList[0].ToList()[4]);
            Assert.IsInstanceOf<IEnumerable<int>>(chunkedNumbersList[1]);
            Assert.AreEqual(5, chunkedNumbersList[1].Count());
            Assert.AreEqual(10, chunkedNumbersList[1].ToList()[4]);
            Assert.IsInstanceOf<IEnumerable<int>>(chunkedNumbersList[2]);
            Assert.AreEqual(3, chunkedNumbersList[2].Count());
            Assert.AreEqual(13, chunkedNumbersList[2].ToList()[2]);
        }

        [Test]
        public void HalfAGroupOfFive()
        {
            var numbers = new int[3]
            {
                1, 2, 3
            };
            var chunkedNumbers = numbers.GroupsOf(5);
            Assert.AreEqual(1, chunkedNumbers.Count());
            var chunkedNumbersList = chunkedNumbers.ToList();
            Assert.IsInstanceOf<IEnumerable<int>>(chunkedNumbersList[0]);
            Assert.AreEqual(3, chunkedNumbersList[0].Count());
        }
    }
}