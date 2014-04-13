using System;
using Cucumber.SimpleDb.Async;
using NUnit.Framework;

namespace Cucumber.SimpleDb.Test.Async
{
    [TestFixture]
    public class SimpleDbAttributeValueEveryTest
    {
        [Test]
        public void StringComparisonNoMatch()
        {
            var stringAttribute = new SimpleDbAttributeValue("hello", "world").Every();
            Assert.IsFalse(stringAttribute == "hello");
            Assert.IsFalse(stringAttribute == "world");
            Assert.IsFalse(stringAttribute == "foo");
        }

        [Test]
        public void StringComparisonYesMatch()
        {
            var stringAttribute = new SimpleDbAttributeValue("hello", "hello").Every();
            Assert.IsTrue(stringAttribute == "hello");
        }

        [Test]
        public void IntComparisonNoMatch()
        {
            var intAttribute = new SimpleDbAttributeValue("1", "2").Every();
            Assert.IsFalse(intAttribute == 1);
            Assert.IsFalse(intAttribute == 2);
            Assert.IsFalse(intAttribute < 2);
            Assert.IsFalse(intAttribute > 1);
            Assert.IsFalse(intAttribute == 3);
        }

        [Test]
        public void IntComparisonYesMatch()
        {
            var intAttribute = new SimpleDbAttributeValue("1", "2").Every();
            Assert.IsTrue(intAttribute <= 2);
            Assert.IsTrue(intAttribute >= 1);
        }

        [Test]
        public void LongComparisonNoMatch()
        {
            var longAttribute = new SimpleDbAttributeValue("4294967296", "4294967297").Every();
            Assert.IsFalse(longAttribute == 4294967296L);
            Assert.IsFalse(longAttribute == 4294967297L);
            Assert.IsFalse(longAttribute < 4294967297L);
            Assert.IsFalse(longAttribute > 4294967296L);
            Assert.IsFalse(longAttribute == 4294967298L);
        }

        [Test]
        public void LongComparisonYesMatch()
        {
            var longAttribute = new SimpleDbAttributeValue("4294967296", "4294967297").Every();
            Assert.IsTrue(longAttribute <= 4294967297L);
            Assert.IsTrue(longAttribute >= 4294967296L);
        }

        [Test]
        public void FloatComparisonNoMatch()
        {
            var floatAttribute = new SimpleDbAttributeValue("3.5", "4.5").Every();
            Assert.IsFalse(floatAttribute == 3.5F);
            Assert.IsFalse(floatAttribute == 4.5F);
            Assert.IsFalse(floatAttribute < 4.5F);
            Assert.IsFalse(floatAttribute > 3.5F);
            Assert.IsFalse(floatAttribute == 5.5F);
        }

        [Test]
        public void FloatComparisonYesMatch()
        {
            var floatAttribute = new SimpleDbAttributeValue("3.5", "4.5").Every();
            Assert.IsTrue(floatAttribute <= 4.5F);
            Assert.IsTrue(floatAttribute >= 3.5F);
        }

        [Test]
        public void DoubleComparisonNoMatch()
        {
            var doubleAttribute = new SimpleDbAttributeValue("4294967296.5", "4294967297.5").Every();
            Assert.IsFalse(doubleAttribute == 4294967296.5D);
            Assert.IsFalse(doubleAttribute == 4294967297.5D);
            Assert.IsFalse(doubleAttribute < 4294967297.5D);
            Assert.IsFalse(doubleAttribute > 4294967296.5D);
            Assert.IsFalse(doubleAttribute == 4294967298.5D);
        }

        [Test]
        public void DoubleComparisonYesMatch()
        {
            var doubleAttribute = new SimpleDbAttributeValue("4294967296.5", "4294967297.5").Every();
            Assert.IsTrue(doubleAttribute <= 4294967297.5D);
            Assert.IsTrue(doubleAttribute >= 4294967296.5D);
        }

        [Test]
        public void DecimalComparisonNoMatch()
        {
            var decimalAttribute = new SimpleDbAttributeValue("300.5", "301.5").Every();
            Assert.IsFalse(decimalAttribute == 300.5m);
            Assert.IsFalse(decimalAttribute == 301.5m);
            Assert.IsFalse(decimalAttribute < 301.5m);
            Assert.IsFalse(decimalAttribute > 300.5m);
            Assert.IsFalse(decimalAttribute == 302.5m);
        }

        [Test]
        public void DecimalComparisonYesMatch()
        {
            var decimalAttribute = new SimpleDbAttributeValue("300.5", "301.5").Every();
            Assert.IsTrue(decimalAttribute <= 301.5m);
            Assert.IsTrue(decimalAttribute >= 300.5m);
        }

        [Test]
        public void DateTimeComparisonNoMatch()
        {
            var dateTimeAttribute = new SimpleDbAttributeValue("5/1/2008 8:30:52AM", "5/2/2008 8:30:52AM").Every();
            Assert.IsFalse(dateTimeAttribute == new DateTime(2008, 5, 1, 8, 30, 52));
            Assert.IsFalse(dateTimeAttribute == new DateTime(2008, 5, 2, 8, 30, 52));
            Assert.IsFalse(dateTimeAttribute < new DateTime(2008, 5, 2, 8, 30, 52));
            Assert.IsFalse(dateTimeAttribute > new DateTime(2008, 5, 1, 8, 30, 52));
            Assert.IsFalse(dateTimeAttribute == new DateTime(2008, 5, 3, 8, 30, 52));
        }

        [Test]
        public void DateTimeComparisonYesMatch()
        {
            var dateTimeAttribute = new SimpleDbAttributeValue("5/1/2008 8:30:52AM", "5/2/2008 8:30:52AM").Every();
            Assert.IsTrue(dateTimeAttribute <= new DateTime(2008, 5, 2, 8, 30, 52));
            Assert.IsTrue(dateTimeAttribute >= new DateTime(2008, 5, 1, 8, 30, 52));
        }
    }
}