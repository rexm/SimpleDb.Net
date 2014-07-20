using NUnit.Framework;
using System;
using System.Linq;
using System.Xml.Linq;
using Cucumber.SimpleDb.Transport;
using Cucumber.SimpleDb.Test.Transport;

namespace Cucumber.SimpleDb.Test
{
    [TestFixture]
    public class AggregateTests
    {
        [Test]
        public void FirstBasic()
        {
            using(var context = SimpleDbContext.Create(
                new SimpleDbRestService(
                    new SimpleInMemoryItemService(2))))
            {
                var item = context.Domains["TestDomain1"].Items.First();
                Assert.IsNotNull(item);
            }
        }

        [Test]
        public void FirstOrDefaultBasic()
        {
            using(var context = SimpleDbContext.Create(
                new SimpleDbRestService(
                    new SimpleInMemoryItemService(2))))
            {
                var item = context.Domains["TestDomain1"].Items.FirstOrDefault();
                Assert.IsNotNull(item);
            }
        }

        [Test]
        public void FirstBasicNoElements()
        {
            using(var context = SimpleDbContext.Create(
                new SimpleDbRestService(
                    new SimpleInMemoryItemService(0))))
            {
                var exception = Assert.Catch(() =>
                    {
                        context.Domains["TestDomain1"].Items.First();
                    });
                Assert.IsInstanceOf<InvalidOperationException>(exception);
            }
        }

        [Test]
        public void FirstOrDefaultNoElements()
        {
            using(var context = SimpleDbContext.Create(
                new SimpleDbRestService(
                    new SimpleInMemoryItemService(0))))
            {
                var item = context.Domains["TestDomain1"].Items.FirstOrDefault();
                Assert.IsNull(item);
            }
        }
    }
}

