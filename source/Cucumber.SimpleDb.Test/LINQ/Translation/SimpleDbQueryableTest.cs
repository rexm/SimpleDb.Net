using System.Linq;
using System.Xml.Linq;
using Cucumber.SimpleDb.Transport;
using NUnit.Framework;

namespace Cucumber.SimpleDb.Test
{
    [TestFixture]
    public class SimpleDbQueryableTest
    {
        [Test]
        public void UsingConsistencyBasic()
        {
            XElement result = null;
            using (var context = SimpleDbContext.Create(
                new SimpleDbRestService(
                    new CaptureArgumentsRestService(x => result = x))))
            {
                context.Domains["TestDomain1"].Items.WithConsistency().Count();
                Assert.IsTrue(result.Elements("Argument")
                    .FirstOrDefault(x =>
                        x.Element("Key").Value == "ConsistentRead" &&
                        x.Element("Value").Value == "True") != null);
            }
        }

        [Test]
        public void UsingConsistencyWhereSource()
        {
            XElement result = null;
            using (var context = SimpleDbContext.Create(
                new SimpleDbRestService(
                    new CaptureArgumentsRestService(x => result = x))))
            {
                context.Domains["TestDomain1"].Items
                    .Where(i => i["Foo"] == "Bar")
                    .WithConsistency().Count();
                Assert.IsTrue(result.Elements("Argument")
                    .FirstOrDefault(x =>
                        x.Element("Key").Value == "ConsistentRead" &&
                        x.Element("Value").Value == "True") != null);
                Assert.AreEqual(
                    "SELECT COUNT(*) FROM `TestDomain1` WHERE `Foo` = \"Bar\"",
                    result.Elements("Argument")
                        .FirstOrDefault(x => x.Element("Key").Value == "SelectExpression")
                        .Element("Value").Value);
            }
        }

        [Test]
        public void UsingConsistencyWhereBounded()
        {
            XElement result = null;
            using (var context = SimpleDbContext.Create(
                new SimpleDbRestService(
                    new CaptureArgumentsRestService(x => result = x))))
            {
                context.Domains["TestDomain1"].Items
                    .WithConsistency()
                    .Where(i => i["Foo"] == "Bar").Count();
                Assert.IsTrue(result.Elements("Argument")
                    .FirstOrDefault(x =>
                        x.Element("Key").Value == "ConsistentRead" &&
                        x.Element("Value").Value == "True") != null);
                Assert.AreEqual(
                    "SELECT COUNT(*) FROM `TestDomain1` WHERE `Foo` = \"Bar\"",
                    result.Elements("Argument")
                        .FirstOrDefault(x => x.Element("Key").Value == "SelectExpression")
                        .Element("Value").Value);
            }
        }
    }
}