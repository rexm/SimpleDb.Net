using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Cucumber.SimpleDb.Async;
using Cucumber.SimpleDb.Async.Session;
using Cucumber.SimpleDb.Async.Transport;
using Cucumber.SimpleDb.Async.Utilities;
using Cucumber.SimpleDb.Test.Async.Transport;
using NUnit.Framework;

namespace Cucumber.SimpleDb.Test.Async.Linq.Translation
{
    [TestFixture]
    public class SimpleDbQueryableTest
    {
        [Test]
        public async Task UsingConsistencyBasic()
        {
            XElement result = null;
            var captureArgumentsRestService = new CaptureArgumentsRestService(x => result = x);
            var simpleDbRestService = new SimpleDbRestService(captureArgumentsRestService);
            using (var context = new SessionSimpleDbContext(simpleDbRestService, new SimpleDbSession(simpleDbRestService)))
            {
                await (context.Domains["TestDomain1"]).Items.WithConsistency().CountAsync();
                Assert.IsTrue(result.Elements("Argument")
                    .FirstOrDefault(x =>
                        x.Element("Key").Value == "ConsistentRead" &&
                        x.Element("Value").Value == "True") != null);
            }
        }

        [Test]
        public async Task UsingConsistencyWhereSource()
        {
            XElement result = null;
            var captureArgumentsRestService = new CaptureArgumentsRestService(x => result = x);
            var simpleDbRestService = new SimpleDbRestService(captureArgumentsRestService);
            using (var context = new SessionSimpleDbContext(simpleDbRestService, new SimpleDbSession(simpleDbRestService)))
            {
                await (context.Domains["TestDomain1"]).Items
                    .Where(i => i["Foo"] == "Bar")
                    .WithConsistency().CountAsync();
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
        public async Task UsingConsistencyWhereBounded()
        {
            XElement result = null;
            var captureArgumentsRestService = new CaptureArgumentsRestService(x => result = x);
            var simpleDbRestService = new SimpleDbRestService(captureArgumentsRestService);
            using (var context = new SessionSimpleDbContext(simpleDbRestService, new SimpleDbSession(simpleDbRestService)))
            {
                await (context.Domains["TestDomain1"]).Items
                    .WithConsistency()
                    .Where(i => i["Foo"] == "Bar").CountAsync();
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