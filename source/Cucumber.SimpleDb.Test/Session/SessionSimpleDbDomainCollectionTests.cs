using NUnit.Framework;
using System;
using System.Linq;
using Cucumber.SimpleDb.Test.Transport;

namespace Cucumber.SimpleDb.Test.Session
{
    [TestFixture]
    public class SessionSimpleDbDomainCollectionTests
    {
        [Test]
        public void EnumerateDomains()
        {
            using(var simpleDb = SimpleDbContext.Create(new StaticSimpleDbRestService()))
            {
                var domainList = simpleDb.Domains.ToList();
                Assert.AreEqual(3, domainList.Count);
                Assert.AreEqual("pictures", domainList[0].Name);
                Assert.AreEqual("documents", domainList[1].Name);
                Assert.AreEqual("contacts", domainList[2].Name);
            }
        }
    }
}

