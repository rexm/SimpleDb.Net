using System;
using NUnit.Framework;
using Cucumber.SimpleDb.Test.Transport;
using System.Collections.Generic;
using System.Linq;

namespace Cucumber.SimpleDb.Test
{
    [TestFixture]
    public class SimpleDbItemCreateTests
    {
        [Test]
        public void CreateNewItemWithMultipleAttributes()
        {
            using(var context = SimpleDbContext.Create(new StaticSimpleDbRestService()))
            {
                var item1 = context.Domains["MyTestDomain"].Items.Add(
                    "item1",
                    new Dictionary<string, SimpleDbAttributeValue>
                    {
                        { "Att1", "string value" },
                        { "Att2", 1.01 }
                    });

                Assert.AreEqual(2, item1.Attributes.Count());
                Assert.AreEqual("string value", (string)item1["Att1"]);
                Assert.AreEqual(1.01, (double)item1["Att2"]);
            }
        }
    }
}

