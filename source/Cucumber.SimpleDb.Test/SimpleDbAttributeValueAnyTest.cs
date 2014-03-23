using System;
using NUnit.Framework;

namespace Cucumber.SimpleDb.Test
{
    [TestFixture]
    public class SimpleDbAttributeValueAnyTest
    {
        [Test]
        public void StringComparison()
        {
            var stringAttribute = new SimpleDbAttributeValue("hello", "world");
            Assert.IsTrue(stringAttribute == "hello");
            Assert.IsTrue(stringAttribute == "world");
            Assert.IsFalse(stringAttribute == "foo");
        }

        [Test]
        public void IntComparison()
        {
            var intAttribute = new SimpleDbAttributeValue("1", "2");
            Assert.IsTrue(intAttribute == 1);
            Assert.IsTrue(intAttribute == 2);
            Assert.IsTrue(intAttribute < 2);
            Assert.IsTrue(intAttribute > 1);
            Assert.IsFalse(intAttribute == 3);
        }

        [Test]
        public void LongComparison()
        {
            var longAttribute = new SimpleDbAttributeValue("4294967296", "4294967297");
            Assert.IsTrue(longAttribute == 4294967296L);
            Assert.IsTrue(longAttribute == 4294967297L);
            Assert.IsTrue(longAttribute < 4294967297L);
            Assert.IsTrue(longAttribute > 4294967296L);
            Assert.IsFalse(longAttribute == 4294967298L);
        }

        [Test]
        public void FloatComparison()
        {
            var floatAttribute = new SimpleDbAttributeValue("3.5", "4.5");
            Assert.IsTrue(floatAttribute == 3.5F);
            Assert.IsTrue(floatAttribute == 4.5F);
            Assert.IsTrue(floatAttribute < 4.5F);
            Assert.IsTrue(floatAttribute > 3.5F);
            Assert.IsFalse(floatAttribute == 5.5F);
        }

        [Test]
        public void DoubleComparison()
        {
            var doubleAttribute = new SimpleDbAttributeValue("4294967296.5", "4294967297.5");
            Assert.IsTrue(doubleAttribute == 4294967296.5D);
            Assert.IsTrue(doubleAttribute == 4294967297.5D);
            Assert.IsTrue(doubleAttribute < 4294967297.5D);
            Assert.IsTrue(doubleAttribute > 4294967296.5D);
            Assert.IsFalse(doubleAttribute == 4294967298.5D);
        }

        [Test]
        public void DecimalComparison()
        {
            var decimalAttribute = new SimpleDbAttributeValue("300.5", "301.5");
            Assert.IsTrue(decimalAttribute == 300.5m);
            Assert.IsTrue(decimalAttribute == 301.5m);
            Assert.IsTrue(decimalAttribute < 301.5m);
            Assert.IsTrue(decimalAttribute > 300.5m);
            Assert.IsFalse(decimalAttribute == 302.5m);
        }

        [Test]
        public void DateTimeComparison()
        {
            var dateTimeAttribute = new SimpleDbAttributeValue("5/1/2008 8:30:52AM", "5/2/2008 8:30:52AM");
            Assert.IsTrue(dateTimeAttribute == new DateTime(2008, 5, 1, 8, 30, 52));
            Assert.IsTrue(dateTimeAttribute == new DateTime(2008, 5, 2, 8, 30, 52));
            Assert.IsTrue(dateTimeAttribute < new DateTime(2008, 5, 2, 8, 30, 52));
            Assert.IsTrue(dateTimeAttribute > new DateTime(2008, 5, 1, 8, 30, 52));
            Assert.IsFalse(dateTimeAttribute == new DateTime(2008, 5, 3, 8, 30, 52));
        }
    }
}