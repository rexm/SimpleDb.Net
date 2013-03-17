using System;
using Cucumber.SimpleDb;
using NUnit.Framework;

namespace Cucumber.SimpleDb.Test
{
	[TestFixture()]
	public class SimpleDbAttributeValueConversionTest
	{
		[Test()]
		public void StringConversion ()
		{
			var stringAttribute = new SimpleDbAttributeValue ("test value");
			Assert.IsTrue (stringAttribute == "test value");
		}

		[Test()]
		public void BooleanConversion ()
		{
			var stringAttribute = new SimpleDbAttributeValue ("True");
			Assert.IsTrue (stringAttribute == true);
		}

		[Test()]
		public void FailedBooleanConversion ()
		{
			var stringAttribute = new SimpleDbAttributeValue ("test value");
			Assert.IsFalse (stringAttribute == true);
		}

		[Test()]
		public void IntConverstion ()
		{
			var intAttribute = new SimpleDbAttributeValue ("1");
			Assert.IsTrue (intAttribute == 1);
		}

		[Test()]
		public void FailedIntConverstion ()
		{
			var intAttribute = new SimpleDbAttributeValue ("test value");
			Assert.IsFalse (intAttribute == 1);
		}

		[Test()]
		public void LongConversion ()
		{
			var longAttribute = new SimpleDbAttributeValue ("4294967296");
			Assert.IsTrue (longAttribute == 4294967296L);
		}

		[Test()]
		public void FailedLongConversion ()
		{
			var longAttribute = new SimpleDbAttributeValue ("test value");
			Assert.IsFalse (longAttribute == 4294967296L);
		}

		[Test()]
		public void FloatConversion ()
		{
			var floatAttribute = new SimpleDbAttributeValue ("3.5");
			Assert.IsTrue (floatAttribute == 3.5F);
		}

		[Test()]
		public void FailedFloatConversion ()
		{
			var floatAttribute = new SimpleDbAttributeValue ("test value");
			Assert.IsFalse (floatAttribute == 3.5F);
		}

		[Test()]
		public void DoubleConversion ()
		{
			var doubleAttribute = new SimpleDbAttributeValue ("4294967296.5");
			Assert.IsTrue (doubleAttribute == 4294967296.5D);
		}
		
		[Test()]
		public void FailedDoubleConversion ()
		{
			var doubleAttribute = new SimpleDbAttributeValue ("test value");
			Assert.IsFalse (doubleAttribute == 4294967296.5D);
		}

		[Test()]
		public void DecimalConversion ()
		{
			var decimalAttribute = new SimpleDbAttributeValue ("300.5");
			Assert.IsTrue (decimalAttribute == 300.5m);
		}
		
		[Test()]
		public void FailedDecimalConversion ()
		{
			var decimalAttribute = new SimpleDbAttributeValue ("test value");
			Assert.IsFalse (decimalAttribute == 300.5m);
		}

		[Test()]
		public void DateTimeConversion ()
		{
			var dateTimeAttribute = new SimpleDbAttributeValue ("5/1/2008 8:30:52AM");
			Assert.IsTrue (dateTimeAttribute == new DateTime(2008, 5, 1, 8, 30, 52));
		}
		
		[Test()]
		public void FailedDateTimeConversion ()
		{
			var dateTimeAttribute = new SimpleDbAttributeValue ("test value");
			Assert.IsFalse (dateTimeAttribute == new DateTime(2008, 5, 1, 8, 30, 52));
		}
	}
}

